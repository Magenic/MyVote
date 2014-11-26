using Cirrious.MvvmCross.ViewModels;
using MyVote.UI.ViewModels;

namespace MyVote.UI 
{ 
    public class App : MvxApplication 
    { 
        public override void Initialize() 
        { 
            RegisterAppStart<LandingPageViewModel>();
        }
    } 
} 