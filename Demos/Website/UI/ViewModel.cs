partial class ViewModel : IViewModel
{
    [Live] string? name;
    [Live] int? count;
    [Live] string? color;
    [Live] bool? showAdditional;
    [Live] SubViewModel? sub;
    public partial struct SubViewModel
    {
        [Live] string? email;
        [Live] string? first;
        [Live] string? last;
        [Live] string? password;
        [Live] int? quantity;


















        public string? Email { get => email; set => email = value; }
        public string? First { get => first; set => first = value; }
        public string? Last { get => last; set => last = value; }
        public string? Password { get => password; set => password = value; }
        public int? Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
            }
        }
    }


    public string? Name
    {
        get
        {
            return name;
        }
        set
        {
            if (!EqualityComparer<string?>.Default.Equals(name, value))
            {
                string? oldValue = name;
                OnNameChanging(value);
                OnNameChanging(oldValue, value);
                OnPropertyChanging();
                name = value;
                OnNameChanged(value);
                OnNameChanged(oldValue, value);
                OnPropertyChanged();
            }
        }
    }

    public int? Count
    {
        get
        {
            return count;
        }
        set
        {
            if (!EqualityComparer<int?>.Default.Equals(count, value))
            {
                int? oldValue = count;
                OnNameChanging(value);
                OnNameChanging(oldValue, value);
                OnPropertyChanging();
                count = value;
                OnNameChanged(value);
                OnNameChanged(oldValue, value);
                OnPropertyChanged();
            }
        }
    }

    public string? Color
    {
        get
        {
            return color;
        }
        set
        {
            if (!EqualityComparer<string?>.Default.Equals(color, value))
            {
                string? oldValue = color;
                OnNameChanging(value);
                OnNameChanging(oldValue, value);
                OnPropertyChanging();
                color = value;
                OnNameChanged(value);
                OnNameChanged(oldValue, value);
                OnPropertyChanged();
            }
        }
    }

    public bool? ShowAdditional
    {
        get
        {
            return showAdditional;
        }
        set
        {
            if (!EqualityComparer<bool?>.Default.Equals(showAdditional, value))
            {
                bool? oldValue = showAdditional;
                OnNameChanging(value);
                OnNameChanging(oldValue, value);
                OnPropertyChanging();
                showAdditional = value;
                OnNameChanged(value);
                OnNameChanged(oldValue, value);
                OnPropertyChanged();
            }
        }
    }

    public SubViewModel? Sub
    {
        get
        {
            return sub;
        }
        set
        {
            if (!EqualityComparer<SubViewModel?>.Default.Equals(sub, value))
            {
                SubViewModel? oldValue = sub;
                OnNameChanging(value);
                OnNameChanging(oldValue, value);
                OnPropertyChanging();
                sub = value;
                OnNameChanged(value);
                OnNameChanged(oldValue, value);
                OnPropertyChanged();
            }
        }
    }

    bool isBatching = false;
    bool isBatchChanged = false;
    public Action? OnChanged { get; set; }

    public ViewModel()
    {
    }

    public IDisposable Batch()
    {
        return new Batchable(this);
    }

    private void OnNameChanging<T>(T value)
    {

    }

    private void OnNameChanging<T>(T oldValue, T newValue)
    {

    }

    private void OnPropertyChanging()
    {

    }

    private void OnNameChanged<T>(T value)
    {

    }

    private void OnNameChanged<T>(T oldValue, T newValue)
    {

    }

    private void OnPropertyChanged()
    {
        if (isBatching)
            isBatchChanged = true;
        else
            OnChanged?.Invoke();

    }

    class Batchable : IDisposable
    {
        readonly ViewModel viewModel;

        public Batchable(ViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.viewModel.isBatching = true;
        }

        public void Dispose()
        {
            viewModel.isBatching = false;
            if (viewModel.isBatchChanged)
                viewModel.OnPropertyChanged();
        }
    }
}