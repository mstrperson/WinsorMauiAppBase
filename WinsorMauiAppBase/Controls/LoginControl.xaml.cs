using WinsorMauiAppBase.Services;

namespace WinsorMauiAppBase.Controls;

public partial class LoginControl : ContentView
{
    public event EventHandler OnLogin;

	public LoginControl()
	{
		InitializeComponent();
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible= false;
        ResponseLabel.IsVisible= false;
        if(EmailEntry.Text == string.Empty) 
        {
            ErrorLabel.Text = "Please enter your email address.";
            ErrorLabel.IsVisible= true;
            return;
        }

        if(PasswordEntry.Text == string.Empty)
        {
            ErrorLabel.Text = "You must enter a password.";
            ErrorLabel.IsVisible = true;
            return;
        }

        try
        {
            ApiService.Login(EmailEntry.Text, PasswordEntry.Text);
        }
        catch (ApiException ae)
        {
            ErrorLabel.Text = ae.Message;
            ErrorLabel.IsVisible = true;
            return;
        }

        ResponseLabel.Text = $"Hello, {ApiService.UserInfo!.firstName}";
        ResponseLabel.IsVisible = true;
        OnLogin?.Invoke(this, e);
    }
}