readonly record struct Hydration() : IView
{
    public HtmlString Render() => $"""
        <section id="zero-hydration">
            <div>
                
            </div>
            <article>
                <h2>
                    Simplify
                </h2>
                <h1>
                    <strong>Zero</strong> hydration
                </h1>
                <p>
                    Xero is a return to good old-fashioned server-side rendering.  
                    Nothing fancy by default.  In fact, if you don't use any 
                    of Xero's "live" features the the technology works the same 
                    as it did in the 90s - just plain old HTML sent over HTTP.
                </p>
                <p>
                    Only when you choose to handle DOM events or include view-model properties 
                    does it mark your "live elements" and open a websocket 
                    connection for bi-directional communication.
                </p>
                <p>
                    This is a large improvement from React-based frameworks since TTFI isn't 
                    blocked while the async content loads.  To put that in other words, if 
                    you have async content you'd like to stream in but might take several 
                    seconds, Xero's users can fully interact with the page while they wait 
                    while React-based users can look but cannot touch until the last content 
                    streams in.
                </p>
                <p>
                    (See Workflowy notes for hydration verbiage used in docs.)
                </p>
            </article>
        </section>
        """;
}
