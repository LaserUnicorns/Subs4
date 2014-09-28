using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Subs4.Common.Classes;
using Subs4.Common.Helpers;
using Subs4.CsvReportReaderLib;
using Subs4.DbfLib;
using Subs4.ReportLib;
using Subs4.XmlPersonsLib;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Subs4.WinApp.Report
{
    public class ReportViewModel : BindableBase
    {
        private readonly CsvReportReader _csvReportReader;
        private readonly DbfDAL _dbfDAL;

        public DelegateCommand LoadCsvReportCommand { get; private set; }
        public DelegateCommand ExportToDbfCommand { get; private set; }
        public DelegateCommand ExportToPdfCommand { get; private set; }
        public DelegateCommand UpdateTotalsCommand { get; private set; }

        private ObservableCollection<Person> _persons;
        public ObservableCollection<Person> Persons
        {
            get { return _persons; }
            private set { SetProperty(ref _persons, value); }
        }

        public ObservableCollection<ServiceTotalsViewModel> ServicesTotals
        {
            get
            {
                var xs = from p in Persons
                         from b in p.Benefits
                         group b by b.ServiceGroupCode
                         into g
                         select new ServiceTotalsViewModel
                                {
                                    ServiceGroupCode = g.Key,
                                    Value = g.Sum(x => x.Value)
                                };
                xs = xs.Append(new ServiceTotalsViewModel {ServiceGroupCode = "Всего", Value = xs.Sum(x => x.Value)});
                return new ObservableCollection<ServiceTotalsViewModel>(xs);
            }
        }

        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set { SetProperty(ref _selectedPerson, value); }
        }

        private static string XmlPersonsFilePath
        {
            get { return ConfigurationManager.AppSettings["XmlPersonsFilePath"]; }
        }

        public ReportViewModel(XmlPersonsDAL xmlPersonsDAL, DbfDAL dbfDAL)
        {
            _csvReportReader = new CsvReportReader(xmlPersonsDAL.GetPersons(XmlPersonsFilePath));
            _dbfDAL = dbfDAL;

            LoadCsvReportCommand = new DelegateCommand(LoadCsvReport);
            ExportToDbfCommand = new DelegateCommand(ExportToDbf, () => Persons.Any());
            ExportToPdfCommand = new DelegateCommand(ExportToPdf, () => Persons.Any());
            UpdateTotalsCommand = new DelegateCommand(UpdateTotals, () => Persons.Any());

            Persons = new ObservableCollection<Person>();
        }

        private void UpdateTotals()
        {
            OnPropertyChanged(() => ServicesTotals);
        }

        private void ExportToPdf()
        {
            var pdfFileName = Path.Combine(Environment.CurrentDirectory, "report.pdf");
            ReportCreator.CreateReport(Persons, pdfFileName);

            Process.Start(pdfFileName);
        }

        private void ExportToDbf()
        {
            var dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _dbfDAL.Connect(dlg.SelectedPath);

                foreach (var person in Persons)
                {
                    _dbfDAL.AddPersonBenefits(person);
                }
            }
        }

        private void LoadCsvReport()
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                var report = _csvReportReader.Load(dlg.FileName);

                Persons = new ObservableCollection<Person>(report.OrderBy(x => x.Address.Flat).ThenBy(x => x.Address.Room));

                OnPropertyChanged(() => ServicesTotals);
            }

            ExportToDbfCommand.RaiseCanExecuteChanged();
            ExportToPdfCommand.RaiseCanExecuteChanged();
            UpdateTotalsCommand.RaiseCanExecuteChanged();
        }
    }
}