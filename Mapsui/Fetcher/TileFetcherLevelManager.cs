using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile;
using System.Net;
using System.Collections.Concurrent;
using System.Globalization;

namespace Mapsui.Fetcher
{
    internal sealed class TileFetcherLevelManager
    {
        private readonly ConcurrentDictionary<string, int> _removedResolutions;
        private readonly ITileSource _tileSource;
        private readonly object _lock = new object();
        private Dictionary<string, Resolution> _resolutions;

        internal TileFetcherLevelManager(ITileSource _tileSource)
        {
            this._tileSource = _tileSource;
            _removedResolutions = new ConcurrentDictionary<string, int>();
        }

        internal IDictionary<string, Resolution> TileResolutions
        {
            get
            {
                lock(_lock)
                {
                    if (_resolutions == null)
                    {
                        // Remove resolutions that has present three or more NotFound (404) errors.
                        _resolutions = _tileSource.Schema.Resolutions
                            .Where(kvp => !(_removedResolutions.ContainsKey(kvp.Key) && _removedResolutions[kvp.Key] > 2))
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    }
                
                    return _resolutions;
                }
            }
        }

        internal void OnFetchCompleted(FetchTileCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                // Reenter levels below the current removed level.
                int indexLevel;
                if (int.TryParse(e.TileInfo.Index.Level, NumberStyles.Integer, CultureInfo.InvariantCulture, out indexLevel))
                {
                    foreach (var removedLevel in Levels(_removedResolutions.Keys))
                    {
                        if (removedLevel <= indexLevel)
                        {
                            int temp;
                            _removedResolutions.TryRemove(removedLevel.ToString(CultureInfo.InvariantCulture), out temp);
                        }
                    }
                }
            }
            else
            {
                var webError = e.Error as WebException;

                if (webError != null)
                {
                    var httpResponse = webError.Response as HttpWebResponse;
                    if (httpResponse != null)
                    {
                        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                        {
                            // Remove resolutions.
                            if (!_removedResolutions.ContainsKey(e.TileInfo.Index.Level))
                                _removedResolutions[e.TileInfo.Index.Level] = 1;
                            else
                                _removedResolutions[e.TileInfo.Index.Level] += 1;

                            lock(_lock)
                            {
                                _resolutions = null;
                            }

                            // Force reload.
                            if (ForceReload != null)
                                ForceReload(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        private IEnumerable<int> Levels(ICollection<string> collection)
        {
            foreach (var @string in collection)
            {
                int indexLevel;
                if (int.TryParse(@string, NumberStyles.Integer, CultureInfo.InvariantCulture, out indexLevel))
                {
                    yield return indexLevel;
                }
            }
        }

        public event EventHandler ForceReload;
    }
}
