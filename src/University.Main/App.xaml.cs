using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Threading;
using University.Data;
using University.Interfaces;
using University.Services;
using University.ViewModels;

namespace University.Main
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UniversityContext>();

                // Delete and recreate the database
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                dbContext.SaveData("data.json"); // Save data to JSON file after database recreation
            }

            MainWindow mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow.Show();

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }


        private void ConfigureServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<UniversityContext>(options =>
            {
                options.UseInMemoryDatabase("UniversityDb");
                options.UseLazyLoadingProxies();
            });
            serviceCollection.AddSingleton<IDialogService, DialogService>();
            serviceCollection.AddSingleton<MainWindowViewModel>();
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddTransient<IClassroomService, ClassroomService>();
        }

        private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.InnerException, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        private void OnProcessExit(object? sender, EventArgs e)
        {
            // Retrieve the UniversityContext from the service provider
            var dbContext = ServiceProvider.GetService<UniversityContext>();
            if (dbContext != null)
            {
                // Save data to JSON file on application exit
                dbContext.SaveData("data.json");
            }
        }

    }
}
