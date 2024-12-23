using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace People.ViewModels
{
    public class PersonViewModel : ObservableObject, IQueryAttributable
    {
        private readonly PersonRepository _personRepository;
        private Models.Person _person;

        public Models.Person Person
        {
            get => _person;
            set
            {
                if (SetProperty(ref _person, value)) //Profe estoy usando este método porque me instale el CommunityToolkit.MVVM
                {
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private ObservableCollection<Models.Person> _peopleList;
        public ObservableCollection<Models.Person> PeopleList
        {
            get => _peopleList;
            set => SetProperty(ref _peopleList, value); 
        }

        public string Name
        {
            get => _person.Name;
            set
            {
                if (_person.Name != value)
                {
                    _person.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Id => _person.Id;

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand GetAllPeopleCommand { get; }

        public PersonViewModel()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "people.db3");
            _personRepository = new PersonRepository(dbPath);

            _person = new Models.Person();
            PeopleList = new ObservableCollection<Models.Person>();
            SaveCommand = new AsyncRelayCommand(Save);
            GetAllPeopleCommand = new AsyncRelayCommand(LoadPeople);
        }

        private async Task Save()
        {
            try
            {
                if (string.IsNullOrEmpty(_person.Name))
                {
                    throw new Exception("El nombre no puede estar vacío.");
                }
                _personRepository.AddNewPerson(_person.Name);

                StatusMessage = $"Persona {_person.Name} guardada exitosamente.";
                await Shell.Current.GoToAsync($"..?saved={_person.Name}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error al guardar la persona: {ex.Message}";
            }
        }

       
        private async Task LoadPeople()
        {
            try
            {
                var people = _personRepository.GetAllPeople();
                PeopleList.Clear();
                foreach (var person in people)
                {
                    PeopleList.Add(person);
                }

                StatusMessage = $"Se cargaron {PeopleList.Count} personas.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error al obtener personas: {ex.Message}";
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("person") && query["person"] is Models.Person person)
            {
                Person = person;
            }
        }
    }
}
