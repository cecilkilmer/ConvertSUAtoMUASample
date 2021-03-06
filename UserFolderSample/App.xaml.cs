﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UserFolderSample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            IAsyncOperation<IReadOnlyList<Windows.System.User>> UserListOperation = SpinWait(Windows.System.User.FindAllAsync());
            IReadOnlyList<Windows.System.User> UserList = UserListOperation.GetResults();
            int UserCount = UserList.Count;
            foreach (var user in UserList)
            {
                var FirstNameAsync = SpinWait(user.GetPropertyAsync("FirstName"));
                var FirstName = FirstNameAsync.GetResults();
                System.Diagnostics.Debug.WriteLine(FirstName + "\n");

                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var dataFile = SpinWait(localFolder.CreateFileAsync("dataFile.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting));

                // This should be await, but it's not an async method
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Windows.Storage.FileIO.WriteTextAsync(dataFile.GetResults(), "User: " + FirstName + "\n");
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Spin wait for async operation to complete.
        /// </summary>
        /// <typeparam name="T">Operation type.</typeparam>
        /// <param name="operation">Operation to wait for.</param>
        /// <returns>Operation passed in (convenience).</returns>
        static private IAsyncOperation<T> SpinWait<T>(IAsyncOperation<T> operation)
        {
            while (operation.Status == AsyncStatus.Started) { }
            return operation;
        }
    }
}
