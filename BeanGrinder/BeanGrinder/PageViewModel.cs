using MLToolkit.Forms.SwipeCardView.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace BeanGrinder
{
    public abstract class BasePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged( [CallerMemberName] string name = null ) => PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
    }

    class PageViewModel : BasePageViewModel
    {
        public class Bean
        {
            public int corporation_id { get; set; }
            public string description { get; set; }
            public string birthday;

            public int character_id;
            public float security_status;

            public string name { get; set; }
            public string Portrait { get { return $"https://images.evetech.net/characters/{character_id}/portrait?size=1024"; } }
            public string CorpLogo { get { return $"https://images.evetech.net/corporations/{corporation_id}/logo?size=256"; } }
            public string Age { get { return birthday.Substring( 0, birthday.IndexOf( '-' ) ); } }
            public string SecStat { get { return security_status.ToString( "0.0" ); } }
        }

        List<Bean> beanQueue = new List<Bean>();

        ObservableCollection<Bean> _beans = new ObservableCollection<Bean>();
        public ObservableCollection<Bean> Beans
        {
            get => _beans;
            set
            {
                _beans = value;
                RaisePropertyChanged();
            }
        }

        List<int> alreadySwiped = new List<int>();

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }

        public PageViewModel()
        {
            DownloadAllBeans();

            SwipedCommand = new Command<SwipedCardEventArgs>( OnSwipedCommand );
        }

        const int phAllianceID = 99005338;
        public class EveWhoData
        {
            public class AllianceInfo
            {
                public int alliance_id;
                public string name;
                public int memberCount;
            }

            public List<AllianceInfo> info;
            public List<Bean> characters;
        }
        EveWhoData allianceData;
        int beanIterator = 0;

        public async void DownloadAllBeans()
        {
            using( var client = new System.Net.Http.HttpClient() )
            {
                var uri = new System.Uri( $"http://evewho.com/api/allilist/{phAllianceID}" );
                var strJSON = string.Empty;
                try
                {
                    strJSON = await client.GetStringAsync( uri );
                }
                catch( System.Exception e )
                {
                    strJSON = e.Message;
                }
                allianceData = Newtonsoft.Json.JsonConvert.DeserializeObject<EveWhoData>( strJSON );
                allianceData.characters.Shuffle( new System.Random() );

                beanQueue = await QueueRandomBeans();
                Beans = new ObservableCollection<Bean>( beanQueue );
                beanQueue = await QueueRandomBeans();
            }
        }

        public ICommand SwipedCommand { get; }

        private void OnSwipedCommand( SwipedCardEventArgs eventArgs )
        {
            var b = eventArgs.Item as Bean;
            var i = Beans.IndexOf( b );
            if( i == Beans.Count - 1 )
            {
                Beans = new ObservableCollection<Bean>( beanQueue );
                WaitForNewBeans();
            }

            var item = eventArgs.Item as string;
            Message = $"{i} swiped {eventArgs.Direction}";
        }

        async void WaitForNewBeans()
        {
            beanQueue = await QueueRandomBeans();
        }

        public async System.Threading.Tasks.Task<List<Bean>> QueueRandomBeans()
        {
            var newBeanQueue = new List<Bean>();

            if( alreadySwiped.Count >= allianceData.characters.Count )
                alreadySwiped.Clear();

            using( var client = new System.Net.Http.HttpClient() )
            {
                for( var i = beanIterator; i < beanIterator + 10; i++ )
                {
                    var charID = allianceData.characters[i].character_id;
                    var uri = new System.Uri( $"https://esi.evetech.net/latest/characters/{charID}/?datasource=tranquility" );
                    var strJSON = await client.GetStringAsync( uri );
                    var newBean = Newtonsoft.Json.JsonConvert.DeserializeObject<Bean>( strJSON );
                    newBean.character_id = charID;

                    newBeanQueue.Add( newBean );
                }
            }

            beanIterator += 10;

            return newBeanQueue;
        }
    }
}
