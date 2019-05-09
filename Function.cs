using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Alexa.NET;
using Alexa.NET.Response;
using Alexa.NET.Response.Ssml;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Linq;
using System.IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AlexaSkillTime
{
    public class Function : ILambdaLogger
    {        
        public int MyProperty { get; set; }
        public void Log(string LoggerMessage)
        {            
        }
        public void LogLine(string LoggerMessage)
        {
        }

        public static void Logger(string LoggerMessage)
        {
            LambdaLogger.Log($"-----------{LoggerMessage}");
        }

        public class StoryResource
        {
            public StoryResource(string language)
            {
                this.Language = language;
            }
            public string Language { get; set; }
            public string SkillName { get; set; }
            public string Landing { get; set; }
            public string GetAlexaSkill { get; set; }
            public string Options { get; set; }
            public string WhatIsAlexaSkill { get; set; }
            public string Unknown { get; set; }
            public string HelpMessage { get; set; }
            public string HelpReprompt { get; set; }
            public string CancelMessage { get; set; }
            public string StopMessage { get; set; }
        }

        #region _workingNotes
        //need to create a "new story" function to allow people to only hear the newest stories
        //maybe create a new story function that allows users to submit stories for review
        //need to add a function to exclude a story, maybe
        //-----> Currently Working On
        //I want the user to be able to pass the name of a story - this method should take that name and return an int, then I can pass that int to StoryChooser
        //https://developer.amazon.com/bLoggers/post/tx3ihsfqsuf3rqp/why-a-custom-slot-is-the-literal-solution
        //https://developer.amazon.com/docs/custom-skills/literal-slot-type-reference.html
        //https://developer.amazon.com/docs/custom-skills/create-and-edit-custom-slot-types.html#add-edit-type
        #endregion

        #region List of Stories

        public static void BuildStoryList(List<string> storyList)
        { //build the list

            //Story 1 - King Daddy and the Crazy Huge Dragon --> Complete
            storyList.Add(@"<speak>King Daddy and the Crazy Huge Dragon. 

                </speak>");

            //Story 2 - Ben and the Giant Kitty
            storyList.Add(@"<speak>Ben and the Giant Kitty.
     
                </speak>");

            //Story 3 - The Kissy Monster
            storyList.Add(@"<speak>The Kissy Monster.
                </speak>");

            //Story 4 - Water Girl and Fire Boy
            storyList.Add(@"<speak>Water Girl and Fire Boy

                </speak>");

            //Story 5 - A Mysterious Case of Pox
            //mystery
            storyList.Add(@"<speak>A Mysterious Case of Chicken Pox
        
                </speak>");
            //see if you can write in an option to tell Alexa what the user wants to try to do from this point on


            //ADD STORIES ABOVE THIS LINE ^^^^^^

            //Returned when users have heard all stories
            storyList.Add(@"<speak>There are no more stories. You can say what is AlexaSkill, start over or exit.</speak>");
        }

        #endregion        

        #region Prosody Ref
        // <prosody rate='x-slow'>This is a test of the x-slow prosody.</prosody>
        // <prosody rate='slow'>This is a test of the slow prosody.</prosody>
        // <prosody rate='medium'>This is a test of the medium prosody.</prosody> 
        // <prosody rate='fast'>This is a test of the fast prosody.</prosody> 
        // <prosody rate='x-fast'>This is a test of the x-fast prosody.</prosody>

        // <prosody pitch='x-low'>This is a test of the x-low prosody.</prosody>
        // <prosody pitch='low'>This is a test of the low prosody.</prosody>
        // <prosody pitch='medium'>This is a test of the medium prosody.</prosody>
        // <prosody pitch='high'>This is a test of the high prosody.</prosody>
        // <prosody pitch='x-high'>This is a test of the x-high prosody.</prosody>

        // <prosody volume='x-soft'>This is a test of the x-slow prosody.</prosody>
        // <prosody volume='soft'>This is a test of the slow prosody.</prosody>
        // <prosody volume='medium'>This is a test of the medium prosody.</prosody>
        // <prosody volume='loud'>This is a test of the fast prosody.</prosody>
        // <prosody volume='x-loud'>This is a test of the x-fast prosody.</prosody>


        #endregion

        public static void BuildPossibleList(List<string> storyList, List<int> possibleList)
        {
            Logger("Building possible list...");
                                //Added -1 to the .Count method to avoid returning the no more stories string
            for (int i = 1; i < storyList.Count - 1; i++)
            {
                possibleList.Add(i);
            }
        }

        public static int GetStoryIndex(string chosenStory)
        {
            Logger("Getting list of story names1...");
            List<string> storyNames = new List<string>();
            storyNames.Add("king daddy");
            storyNames.Add("Ben and the giant kitty");
            storyNames.Add("the kissy monster");
            storyNames.Add("water girl and fire boy");

            int index = storyNames.IndexOf(chosenStory);            

            //WHEN YOU CREATE A NEW STORY, YOU MUST DO THE BELOW TO ADD TO THE CHOOSESTORY FUNCTION
            //Add name of story to storyNames - this is case sensitive --> the name you add to storyNames must match what Alexa returns from the user - Alexa will capitalize nouns
            //Alexa Developer Console > Intents > Slot Types > STRING > Enter a new value > lower case name of story that is added to storyNames list > possible synonyms

            return index;
        }

        public static int StoryChooser(List<string> storyList, List<int> possibleList)
        { //choose the story  
            Logger("Choosing story...");
            int chosenStoryListIndex = 0;

                Random r = new Random();
                if (possibleList.Count > 0)
                {
                    int possibleListIndex = r.Next(possibleList.Count);
                    chosenStoryListIndex = possibleList[possibleListIndex];
                    possibleList.RemoveAt(possibleListIndex);
                }
            return chosenStoryListIndex;
        }

        public static string ReturnStory(List<string> storyList, List<int> possibleList)
        { //print the chosen story
            Logger("Returning story to voice engine...");
            return storyList[StoryChooser(storyList, possibleList)];
        }

        public List<StoryResource> GetResources()
        {
            Logger("Collecting resources...");
            List<StoryResource> resources = new List<StoryResource>();
            StoryResource enUSResource = new StoryResource("en-US")
            {
                SkillName = @"AlexaSkill",
                Landing = @"<speak>Welcome to AlexaSkill! Say play a AlexaSkill, what can I say, or exit</speak>",
                Options = @"<speak>Saying tell me a AlexaSkill will play a random story. Saying choose a story followed by the name of a story will play a specific story. 
                            If you play a random story, AlexaSkill will not play the same story twice this session. Saying what is AlexaSkill will tell you about the app. What do you want to do?</speak>",
                WhatIsAlexaSkill = @"<speak>AlexaSkill is a story time skill.
                    Daddy always told his daughter crazy exaggerated Once Upon A Time stories. When she was about 2 and a half years old, she asked Daddy to tell her a AlexaSkill. 
                    She couldn't say once upon a time right. Even though she's 8 years old today, she still says AlexaSkill. 
                    And Daddy still tells her his crazy stories. Now the whole family loves AlexaSkill, and Daddy hopes your family will too.</speak>",

                Unknown = "We're sorry. AlexaSkill was unable to understand your request. You can say play AlexaSkill, help, what is AlexaSkill, or exit",
                HelpMessage = "You can say play AlexaSkill, help, what is AlexaSkill, or exit... What can I help you with?",
                HelpReprompt = "You can say play AlexaSkill, help, what is AlexaSkill, or exit",
                CancelMessage = "Okay. You can say play AlexaSkill, help, what is AlexaSkill, or exit",
                StopMessage = "Fare thee well friends!"
            };
            resources.Add(enUSResource);
            return resources;
        }

        public SkillResponse FunctionHandler(SkillRequest input)
        {
            Logger("Inside main function...");
            Logger("Logging requested input below...");
            Logger(JsonConvert.SerializeObject(input));

            List<string> storyList = new List<string>();

            BuildStoryList(storyList);

            List<int> possibleList = input.Session.GetPossibleList();
            if (possibleList == null)
            {
                possibleList = new List<int>();
                BuildPossibleList(storyList, possibleList);
            }

            SkillResponse response = new SkillResponse
            {
                Response = new ResponseBody
                {
                    ShouldEndSession = false
                }
            };

            var allResources = GetResources();
            var resource = allResources.FirstOrDefault();
            var player = new PlainTextOutputSpeech();
            var ssmlPlayer = new SsmlOutputSpeech();

                 //https://github.com/timheuer/alexa-skills-dotnet#handling-the-intentrequest

            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                Logger($"Default LaunchRequest made: 'Alexa, play AlexaSkillTime");
                player.Text = null;
                ssmlPlayer.Ssml = resource.Landing;
            }
            else if (input.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;

                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                        Logger($"AMAZON.CancelIntent: send StopMessage");
                        //player.Text = resource.StopMessage;
                        ssmlPlayer.Ssml = resource.CancelMessage;
                        break;
                    case "AMAZON.StopIntent":
                        Logger($"AMAZON.StopIntent: send StopMessage");
                        player.Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        Logger($"AMAZON.HelpIntent: send HelpMessage");
                        player.Text = resource.HelpMessage;
                        break;
                    case "AlexaSkillTime": //main story call
                        Logger($"AlexaSkillIntent sent: send New AlexaSkill");
                        ssmlPlayer.Ssml = ReturnStory(storyList, possibleList);
                        break;
                    case "WhatIsAlexaSkill": //tell the user about the app
                        Logger($"AlexaSkillIntent sent: send WhatIsAlexaSkill");
                        ssmlPlayer.Ssml = resource.WhatIsAlexaSkill;
                        break;
                    case "GoAgain": //play another story
                        Logger($"AlexaSkillIntent sent: send New AlexaSkill");
                        ssmlPlayer.Ssml = ReturnStory(storyList, possibleList);
                        break;
                    case "SkipStory":
                        Logger($"AlexaSkillIntent sent: skip story");
                        ssmlPlayer.Ssml = ReturnStory(storyList, possibleList);
                        break;
                    case "ChooseStory": //allow user to choose story by name
                        Logger($"AlexaSkillIntent sent: choose Story");
                        var chosenStory = intentRequest.Intent.Slots["storyChoice"].Value;
                        Logger($"ChooseStory slot result: " + chosenStory);
                        int storyInt = GetStoryIndex(chosenStory);
                        Logger($"ChooseStory -- story index chosen -> " + storyInt);
                        ssmlPlayer.Ssml = storyList[storyInt];
                        break;
                    case "StartOver":
                        Logger($"AlexaSkillIntent sent: reset possibleList");
                        BuildPossibleList(storyList, possibleList);
                        ssmlPlayer.Ssml = ReturnStory(storyList, possibleList);
                        break;
                    case "AlexaSkillOptions":
                        Logger($"AlexaSkillIntent sent: AlexaSkill options");
                        ssmlPlayer.Ssml = resource.Options;
                        break;

                    default:
                        Logger($"Unknown intent: " + intentRequest.Intent.Name);
                        ssmlPlayer.Ssml = resource.HelpMessage;
                        break;
                }
            }

            if (response.SessionAttributes == null)
            {
                response.SessionAttributes = new Dictionary<string, object>();
            }
            response.SessionAttributes["possibleList"] = possibleList;
            if (!string.IsNullOrWhiteSpace(player.Text))
            {
                response.Response.OutputSpeech = player;
            }
            else if (!string.IsNullOrWhiteSpace(ssmlPlayer.Ssml))
            {
                response.Response.OutputSpeech = ssmlPlayer;
            }
            response.Version = "1.0";
            return response;
        }
    }

    public static class Extensions
    {
        public static List<int> GetPossibleList(this Session source)
        {
            List<int> possibleList = null;
            if (source.Attributes == null)
            {
                source.Attributes = new Dictionary<string, object>();
            }

            if (source.Attributes.TryGetValue("possibleList", out object sessionValue))
            {
                var array = (Newtonsoft.Json.Linq.JArray)sessionValue;
                possibleList = array.ToObject<List<int>>();
            }
            return possibleList;
        }
    }
}


#region _lostStoriesRepo
//Story 5 - Sci Fi Story
//simplify, make charming, less grim, dark is okay, but charmingly dark rather than end of the world dark
//storyList.Add(@"<speak> Becky's Forest
//                The world burned. Endless consumption lead to devastating pollution. The environment spiraled out of control until there was nothing left but ash. A little girl dressed in rags tucks herself beneath a bridge to escape the rain. Her name is Becky and she will save the planet.
//                The earth was a barren place now. Very few plants grew at all anywhere at all. But Becky had a secret. She found a tiny tree growing in an abandoned house one day. She nurtured the little plant, sharing every small drink of water she found with it and making sure it had as much compost as she could scounge together.
//                Becky scavenged for years and as she grew, the little tree grew with her. Once the tree was too big for the pot she had it in, she planted it in the center of the courtyard outside of a new home she had built inside an abandonded building.
//                She continued to water the tree every chance she got. And she built little drain gutters from all the buildings surrounding the courtyard so the tree got all the water it could from the few rains that came.
//                Becky grew older and older and the tree grew older and older with her. One day Becky was napping under the now towering tree and something hit her in the head. It was a pine cone that had dropped from the tree. Becky broke the cone open and took the seed out of it.
//                Over the next few months more and more pine cones dropped off the tree. Soon Becky had a giant collection of the cones. She decided to open them up and plant all the seeds from them in the courtyard. She built a series of ditches designed to water all the trees.
//                She moved her gutters around so the ditches would all get water with every little rain that came through.
//                Becky grew older and older. She met a man and his daughter and they fell in love. The three of them continued to grow.
//                Finally Becky was an old lady. Every morning she rose from her bed and walked to her window. She would smile down at the children playing in the courtyard, surrounding by towering pine trees.
//                But, sadly, all things must die. And Becky did.
//                Time progressed ever forward. Becky's family grew older and grew larger and larger. People took notice of Becky's Forest and began to move to her courtyard. Within just few decades Becky's makeshift shack became a bustling town which brimmed with activity.
//                And as time moved forward, more and more trees were planted and grew. The rains began to fall more often and last longer. The forest got larger and larger. Over the years the residents of Becky's Forest moved away and took pine cones with them. They planted the trees like Becky and loved them just the same.
//                Then after one hundred years Becky's great great grand daughter stood on top of a cliff overlooking the courtyard. From it she could see for miles and miles. The grand daughter did not see what Becky saw. She saw a sea of green for as far as she could see. Becky's one little tree had grown to be a vast forest.
//                After three hundred years much of earth was covered in forests again. New species of trees popped up everywhere. People were able to grow crops and feed themselves off the land. The world became a beautiful place again.
//                And the people of this new world told stories about the barren wasteland the world once was. The most popular story was of a little girl who found a single living sapling and raised it to be a towering pine that would eventually save the world.


//                </speak>");

#endregion
