using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace People.ViewModels
{
    public class PersonViewModel : ObservableObject, IQueryAttributable
    {
        private ObservableCollection<Models.Person> _peopleList;
        private string _statusMessage;
        private readonly PersonRepository _personRepository;
        public ICommand SaveCommand { get; }
        public ICommand GetAllPeopleCommand { get; }
        public ICommand DeletePersonCommand { get; }


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

        
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        

        public PersonViewModel()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "people.db3");
            _personRepository = new PersonRepository(dbPath);

            _person = new Models.Person();
            PeopleList = new ObservableCollection<Models.Person>();
            SaveCommand = new AsyncRelayCommand(Save);
            GetAllPeopleCommand = new AsyncRelayCommand(LoadPeople);
            DeletePersonCommand = new AsyncRelayCommand<Models.Person>((person)=>Eliminar(person));
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

        private async Task Eliminar(Models.Person personaAEliminar)
        {
            try
            {
                if (personaAEliminar == null)
                {
                    throw new Exception("Persona no válida.");
                }

                _personRepository.EliminarPersona(personaAEliminar.Name);
                PeopleList.Remove(personaAEliminar);
                StatusMessage = $"Se eliminó a {personaAEliminar.Name}.";

                await Shell.Current.DisplayAlert("Aviso!",$"Gabriel Calderón acaba de eliminar a {personaAEliminar.Name}","Aceptar");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error al eliminar a la persona: {ex.Message}";
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
            else if (query.ContainsKey("deleted"))
            {
                string nombre = query["deleted"].ToString();
                Models.Person matchedPerson = PeopleList.FirstOrDefault(p => p.Name == nombre);

                if (matchedPerson != null)
                    PeopleList.Remove(matchedPerson);
            }
        }
    }
}
