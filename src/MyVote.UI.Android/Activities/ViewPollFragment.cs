using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects;
using Autofac;
using MyVote.UI.NavigationCriteria;
using System.Threading.Tasks;
using BarChart;
using Csla;

namespace MyVote.UI.Droid
{
	public class ViewPollFragment : MyVoteFragment
	{
        [Inject]
        public IObjectFactory<IPoll> PollFactory { get; set; }

        public IPollDataResults _PollDataResults;

        public event EventHandler DeletePressed;
        public event EventHandler ClosePressed;

        private BarChartView _BarChart;

        public IPollDataResults Results {get; set;}

		public ViewPollPageNavigationCriteria NavigationCriteria { get; set; }
		public override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		}

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            MyVoteActivity.setLoadingMessage("Loading poll data...");
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.pollresults, container, false);

            var closeButton = view.FindViewById<Button>(Resource.Id.closeVoteButtonn);
            closeButton.Click += (sender, e) =>
            {
                this.ClosePressed.SafeInvoke(this, EventArgs.Empty);
            };

            var deleteButton = view.FindViewById<Button>(Resource.Id.deletePollButtonn);
            deleteButton.Click += (sender, e) =>
            {
                this.DeletePoll();
            };

            _BarChart = view.FindViewById<BarChartView>(Resource.Id.barChart);
            _BarChart.BarWidth = 100;
            _BarChart.BarOffset = 800 / Results.Results.Count();
            _BarChart.SetClipChildren(false);
            _BarChart.ItemsSource = Results.Results.Select((r) => new BarModel { Value = r.ResponseCount, Legend = r.OptionText, ValueCaption = r.OptionText });

            return view;
        }

        public void DeletePoll()
        {
            this.MessageBox.ShowYesNo(Activity,
                                       "Are you sure you want to delete this poll?", 
                                      async ()=>{
                                           var poll = await this.PollFactory.FetchAsync(this.NavigationCriteria.PollId);
                                           try{
                                                poll.Delete();
                                                await poll.SaveAsync();

                                                this.ClosePressed.SafeInvoke(this, EventArgs.Empty);
                                            }catch(Exception e)
                                            {
                                                View.Post(()=>this.MessageBox.Show(Activity, "Only the poll owner may delete the poll."));
                                                                         
                                            }
                                        },
                                      ()=>{});
        }
	}
}

