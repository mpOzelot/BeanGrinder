using MLToolkit.Forms.SwipeCardView;
using MLToolkit.Forms.SwipeCardView.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BeanGrinder
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new PageViewModel();
        }

        private void OnDislikeClicked( object sender, EventArgs e )
        {
            //SwipeCardView.InvokeSwipe( SwipeCardDirection.Left );
        }

        private void OnInfoButtonClicked( object sender, EventArgs e )
        {
            //SwipeCardView.InvokeSwipe( SwipeCardDirection.Up );
        }

        private void OnLikeClicked( object sender, EventArgs e )
        {
            //SwipeCardView.InvokeSwipe( SwipeCardDirection.Right );
        }
    }
}
