namespace MediaTracker
    {
        using System;
        using System.Net;

        /// <summary>
        ///     Extended the WebClient class to set a custom timeout
        /// </summary>
        public class TimedWebClient : WebClient
            {
                /// <summary>
                ///     Initializes a new instance of the <see cref="TimedWebClient" /> class.
                /// </summary>
                public TimedWebClient() { Timeout = 600000; }

                /// <summary>
                ///     Gets or sets the timeout in msecs.
                /// </summary>
                /// <value>
                ///     The timeout. Default is 600,000 msecs.
                /// </value>
                public int Timeout { get; set; }

                /// <summary>
                ///     Returns a <see cref="T:System.Net.WebRequest" /> object for the specified resource.
                /// </summary>
                /// <param name="address">A <see cref="T:System.Uri" /> that identifies the resource to request.</param>
                /// <returns>
                ///     A new <see cref="T:System.Net.WebRequest" /> object for the specified resource.
                /// </returns>
                protected override WebRequest GetWebRequest(Uri address)
                    {
                        var objWebRequest = base.GetWebRequest(address);
                        objWebRequest.Timeout = Timeout;
                        return objWebRequest;
                    }
            }
    }