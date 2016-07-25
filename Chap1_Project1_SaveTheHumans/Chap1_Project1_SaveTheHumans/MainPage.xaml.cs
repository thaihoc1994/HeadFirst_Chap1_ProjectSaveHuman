using Chap1_Project1_SaveTheHumans.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Xaml.Media.Animation;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Chap1_Project1_SaveTheHumans
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Random random = new Random();
        //Fields, chapter 4
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        bool humanCaptured = false;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();

            enemyTimer.Tick += enemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(2);
            targetTimer.Tick += targetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(0.5);

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        void targetTimer_Tick(object sender, object e)
        {
            progressBar.Value += 1;
            if (progressBar.Value>=progressBar.Maximum)
            {
                EndTheGame();
            }
        }

        private void EndTheGame()
        {
            if (!canPlayArea.Children.Contains(txtGameOver))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                humanCaptured = false;
                btnStart.Visibility = Visibility.Visible;
                canPlayArea.Children.Add(txtGameOver);
            }
        }

        //event handler chapter 15
        void enemyTimer_Tick(object sender, object e)
        {
            AddEnemy(); //them moi enenmy sau 1 fromseconds
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //AddEnemy();
            StartGame();
        }

        private void StartGame()
        {
            human.IsHitTestVisible = true;//chapter 15
            humanCaptured = false;
            progressBar.Value = 0;
            btnStart.Visibility = Visibility.Collapsed;
            canPlayArea.Children.Clear();
            canPlayArea.Children.Add(target);//retanglance
            canPlayArea.Children.Add(human);
            enemyTimer.Start();
            targetTimer.Start();//run till the progressbar full
        }

        private void AddEnemy()
        {
            ContentControl enemy = new ContentControl();
            enemy.Template = Resources["EnemyTemplate"] as ControlTemplate;
            AnimateEnemy(enemy,0,canPlayArea.ActualWidth-100,"(Canvas.Left)");
            AnimateEnemy(enemy,random.Next((int)canPlayArea.ActualHeight - 100),
                random.Next((int)canPlayArea.ActualHeight-100), "(Canvas.Top)");
            canPlayArea.Children.Add(enemy);

            enemy.PointerEntered += enemy_PointerEntered;
            //throw new NotImplementedException();
        }

        void enemy_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
            {
                EndTheGame();
            }
        }

        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimate)
        {
            //animation in chap 16
            //this code make enemy move, if 4,66 => move faster or slower
            Storyboard storyBoard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(2,2)))
            };
            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            storyBoard.Children.Add(animation);
            storyBoard.Begin();
        }

        private void human_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (enemyTimer.IsEnabled)
            {
                humanCaptured = true;
                human.IsHitTestVisible = false;
            }
        }

        private void target_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (targetTimer.IsEnabled && humanCaptured)
            {
                progressBar.Value = 0;
                Canvas.SetLeft(target, random.Next(100, (int)canPlayArea.ActualWidth - 100));
                Canvas.SetTop(target, random.Next(100, (int)canPlayArea.ActualHeight - 100));
                Canvas.SetLeft(human, random.Next(100, (int)canPlayArea.ActualWidth - 100));
                Canvas.SetTop(human, random.Next(100, (int)canPlayArea.ActualWidth - 100));

                humanCaptured = false;
                human.IsHitTestVisible = true;

            }
        }

        private void canPlayArea_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
            {
                Point pointerPosition = e.GetCurrentPoint(null).Position;//<==
                Point releativePosition = grid.TransformToVisual(canPlayArea).TransformPoint(pointerPosition);
                if ((Math.Abs(releativePosition.X -Canvas.GetLeft(human)) > human.ActualWidth*3) || (Math.Abs(releativePosition.Y -Canvas.GetTop(human)) > human.ActualHeight*3))
                {
                    humanCaptured = false;
                    human.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(human,releativePosition.X - human.ActualWidth/2);
                    Canvas.SetTop(human, releativePosition.Y - human.ActualHeight/2);
                }
            }
        }

        private void canPlayArea_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
            {
                EndTheGame();
            }
        }

    }
}
