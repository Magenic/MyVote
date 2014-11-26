
using Xamarin.Forms;

namespace MyVote.UI.Forms
{
    public partial class Login : ContentPageBase
    {
        public Login()
        {
            InitializeComponent();

            this.ResolveAutofacDependencies();

            //Issue with PCL, need to reference something in Xamarin.Forms.Labs or it gets compiled out
            var imageButton = btnMicrosoft;

            //ContentGrid.BackgroundColor = Color.Transparent;
        }
    }
}