using Agents;
using ClimaticsLang;
using ClimaticsLang.Lang;
using Items;
using ItemsLang.Lang;
using Languager;
using Languager.Extensions;
using Mapping;
using MappingLang;
using Outputer;
using Outputer.Choicing;
using Rolling;
using StateMachine;
using Stories;
using Stories.Builders;
using Stories.Playground;
using Stories.Playground.Lang;
using Worlding;

const Language lang = Language.ES;
configureDictionary();

var historic = new Historic();
var world = new World(MachineBuilder.Create()
    .WithState("Initial")
    .EndState()
    .Build());

var timeDescriptor = new TimeDescriptor(world.Time);
Console.WriteLine(timeDescriptor.RandomExternalDescription);

var agent = new StoriesAgent("tobias", "Tobias", "Andrion", Importance.Main);
world.Agents.Add(agent);

var mapped = new StoriesMapped("place", Externality.Internal, new Articler(Genere.Femenine, Number.Singular));
var room = new StoriesMapped("room", Externality.Internal, new Articler(Genere.Femenine, Number.Singular));
world.Map.Add(mapped);
world.Map.Add(room);
world.Map.Connect(mapped, room, Direction.East_West);
world.Map.Ubicate(agent, mapped);

var storylet = StoryletBuilder.Create("test")
    .BeingGlobalSingle()
    .ForMachines()
    .WithAgentsScope()
    .WithInteraction((post) => Output.FromTexts("ejecucion_1".trans()))
        .WithSubinteraction((post) => Output.FromTexts("ejecucion_11".trans()))
            .WithSubinteraction((post) => Output.FromTexts("ejecucion_111".trans()))
            .Build()
            .WithSubinteraction((post) => Output.FromTexts("ejecucion_112".trans()))
            .Build()
        .Build()
        .WithSubinteraction((post) => Output.FromTexts("ejecucion_12".trans()))
            .WithSubinteraction((post) => Output.FromTexts("ejecucion_121".trans()))
            .Build()
            .WithSubinteraction((post) => Output.FromTexts("ejecucion_122".trans()))
            .Build()
        .Build()
    .SetAsRoot()
    .Finish();

var movement = StoryletBuilder.Create("movement")
    .BeingGlobalSingle()
    .ForMachines()
    .WithDescriptor("place")
    .WithPreconditions((pre) => pre.IsExit("place"))
    .WithInteraction((post) =>
    {
        var main = post.Main;
        var origin = post.MainPlace;
        var destination = post.Mapped("place");
        var movementResult = world.Map.Move(main, origin, destination, world.Items);

        return Output.FromTexts("movement_execution".trans(main.Name, new MappedDescriptor(destination).ArticledName));
    })
    .WithDriver(Descriptor.MainRole)
    .SetAsRoot()
    .Finish();

var vault = new StoriesVault(storylet, movement);
var deck = new StoriesDeck();
deck.IncorporateVault(vault);

var deckSelection = deck.GetValidStories(world, historic);
var rolledStories = deckSelection.GetValidStories(agent, world, historic);

var choices = rolledStories.Choices(world.Existents);

Console.WriteLine(choices.ToString());
string line = Console.ReadLine()!;
var input = new Input(int.Parse(line));

var selectedOption = choices.Select(input);

var rolledStorylet = (RolledStorylet)selectedOption.Function();

ExecuteStory(rolledStorylet.Execute(world, historic));

void configureDictionary()
{
    var dictionary = new Dictionary(lang);
    dictionary.Load(new PlaygroundDictionaryProvider());
    dictionary.Load(new ItemsDictionaryProvider());
    dictionary.Load(new ClimaticsDictionaryProvider());

    Translator.Instance.Dictionary = dictionary;
}

void ExecuteStory(IStory story)
{
    var input = Input.Void;
    var step = story.Interact(input);
    Console.WriteLine($"[{story.Driver.Name}]");
    Console.WriteLine(step);
    while (!step.IsEnding)
    {
        Console.WriteLine(step.Choices);

        var inputCommand = Console.ReadLine();
        input = new Input(int.Parse(inputCommand!));
        step = story.Interact(input);
        Console.WriteLine($"[{story.Driver.Name}]");
        Console.WriteLine(step);
    }
}