using System;
using System.Collections.Generic;
using Limbo.Umbraco.Video.Models.Credentials;

namespace Limbo.Umbraco.TwentyThree.Models.Credentials {

    /// <summary>
    /// Class with information about the credentials used for accessing the TwentyThree API.
    /// </summary>
    public class TwentyThreeCredentials : ICredentials {

        /// <summary>
        /// Gets the key of the credentials.
        /// </summary>
        public Guid Key { get; internal set; }

        /// <summary>
        /// Gets the friendly name of the credentials.
        /// </summary>
        public string Name { get; internal set; } = null!;

        /// <summary>
        /// Gets the description of the credentials.
        /// </summary>
        public string? Description { get; internal set; }

        /// <summary>
        /// If configured, gets the domains of the associated TwentyThree site.
        /// </summary>
        public IReadOnlyList<string> Domains { get; internal set; } = null!;

        /// <summary>
        /// If configured, gets the TwentyThree consumer key.
        /// </summary>
        public string? ConsumerKey { get; internal set; }

        /// <summary>
        /// If configured, gets the TwentyThree consumer secret.
        /// </summary>
        public string? ConsumerSecret { get; internal set; }

        /// <summary>
        /// If configured, gets the TwentyThree access token.
        /// </summary>
        public string? AccessToken { get; internal set; }

        /// <summary>
        /// If configured, gets the TwentyThree access token secret.
        /// </summary>
        public string? AccessTokenSecret { get; internal set; }

        /// <summary>
        /// If configured, gets the URL of the upload page.
        /// </summary>
        public string? UploadUrl { get; internal set; }

        /// <summary>
        /// Initializes a new instance with default options.
        /// </summary>
        public TwentyThreeCredentials() { }

    }

}