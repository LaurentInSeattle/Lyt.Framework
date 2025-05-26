namespace Lyt.Mvvm;

public class DialogBindable<TControl, TParameters> : ViewModel<TControl>
    where TControl : class, IView, new()
{
    protected readonly IDialogService dialogService;

    protected Action<DialogBindable<TControl, TParameters>, bool>? onClose;

    protected TParameters? parameters;

    public DialogBindable() : base()
        => this.dialogService = this.Host.Services.GetRequiredService<IDialogService>();

    public DialogBindable(TControl control) : base()
    {
        this.dialogService = this.Host.Services.GetRequiredService<IDialogService>();
        this.Bind(control);
    }

    public virtual void Initialize(
        Action<DialogBindable<TControl, TParameters>, 
        bool>? onClose,
        TParameters? parameters)
    {
        this.onClose = onClose;
        this.parameters = parameters;
    }

    public override bool Validate() => true;

    public override bool TrySaveAndClose()
    {
        bool isValid = this.Validate();
        if (isValid)
        {
            this.onClose?.Invoke(this, true);
            this.dialogService.Dismiss();
        }

        return isValid;
    }

    public override void CancelViewModel() => this.onClose?.Invoke(this, false);

    public override void Cancel()
    {
        this.CancelViewModel();
        this.dialogService.Dismiss();
    }
}
