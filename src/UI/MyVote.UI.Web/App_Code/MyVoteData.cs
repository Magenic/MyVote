using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVote.Client.Web
{
    public static class MyVoteData
    {
        public static IEnumerable<Tuple<string, string>> GetCategories()
        {
            return (new[] {"Fun", "Technology", "Entertainment", "News", "Sports", "Off-Topic"})
                .Select((name, index) => Tuple.Create((index + 1).ToString("0"), name));
        }
    }
}