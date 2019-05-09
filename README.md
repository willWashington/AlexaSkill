# AlexaSkill
An Alexa skill I have been working on for quite a while now that has been scrubbed of my IP.

I began writing this skill with a boilerplate. The boilerplate contained the following:

```
namespace AlexaSkillTime
{
    public class Function : ILambdaLogger



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



public List<StoryResource> GetResources()
        {
    
-^most of this method was already there - I added my own IP


 public SkillResponse FunctionHandler(SkillRequest input)
        {
-^added my own customization to this method
```
I wrote the rest of this document with the support of my mentor Lance Hilliard.
