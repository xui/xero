partial class UI
{
    View Seo(ViewModel vm) => $"""
        <section>
            <div>
                <button onclick="{FetchHtml}">GET https://xero.wtf</button>
            </div>
            <article>
                <h2>
                    Simplify
                </h2>
                <h1>
                    <strong>Zero</strong> SEO headaches
                </h1>
                <p>
                    SEO complicates your web app.  SSR, SSG, ISG and CSF are all complexity 
                    multipliers on your codebase.  Xero returns back to the basics...
                </p>
                <p>
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod
                    tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam,
                    quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
                    consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse
                    cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat
                    non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                </p>
                <p>
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod
                    tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam,
                    quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
                    consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse
                    cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat
                    non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                </p>
            </article>
        </section>
        """;

    void FetchHtml(Context context)
    {
    }
}
