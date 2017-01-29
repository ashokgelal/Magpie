using System.Collections.Generic;
using System.Linq;
using Magpie.Interfaces;
using Magpie.Models;

namespace Magpie.Services
{
    /// <summary>
    /// Class responsible for finding the best channel to check updates against.
    /// </summary>
    internal class BestChannelFinder
    {
        private readonly IDebuggingInfoLogger _logger;

        public BestChannelFinder(IDebuggingInfoLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Finds the best channel less than or equals to the given id and among the matches
        /// returns the best based on the highest version. If there is a tie because of
        /// similar versions, returns the most stable version (i.e. selects a lower id channel).
        /// If no channel matches the id, it returns the channel with the highest version.
        /// </summary>
        /// <param name="id">Id of channel to check against.</param>
        /// <param name="channels">Channels to find the best channel from.</param>
        /// <returns></returns>
        internal Channel Find(int id, List<Channel> channels)
        {
            if (channels == null || !channels.Any())
            {
                _logger.Log("No channels found for selection!");
                return null;
            }

            var validChannels = channels.Where(channel => channel.Id <= id).ToList();
            _logger.Log("Valid channels to select from: " + validChannels.Count);
            var channelToBeSelected = validChannels.FirstOrDefault();
            if (channelToBeSelected == null)
            {
                _logger.Log("No valid channel found for selection!");
                validChannels = channels;
                channelToBeSelected = channels.First();
            }

            foreach (var channel in validChannels)
            {
                if (channel.Version.IsHigherThan(channelToBeSelected.Version))
                {
                    channelToBeSelected = channel;
                }
            }
            _logger.Log("Channel to be selected for checking updates: " + channelToBeSelected);
            return channelToBeSelected;
        }
    }
}