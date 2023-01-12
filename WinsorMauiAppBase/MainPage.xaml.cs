using WinsorMauiAppBase.Controls;

namespace WinsorMauiAppBase;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}
    private void LoginControl_OnLogin(object sender, EventArgs e)
    {
        if (sender is LoginControl login)
        {
            login.IsVisible = false;
        }
    }
}

