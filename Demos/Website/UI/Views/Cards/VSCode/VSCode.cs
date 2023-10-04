partial class UI
{
    View VSCode() => $$"""
        <section>
            <pre>
                <code>
                    partial class UI
                    {
                        View VSCode(ViewModel viewModel) => $"
                            <button onclick="{Increment}">
                                Clicks: {viewModel.Count}
                            </button>
                        ";

                        void Increment(Context ctx)
                        {
                            ctx.ViewModel.Count++;
                        }
                    }
                </code>
            </pre>
        </section>
        """;
}
