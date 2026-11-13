# Cybersecurity Awareness Bot — Part 2

## Project overview

This C# application teaches users about cybersecurity through a
Windows Forms chatbot.

Part 2 extends the Part 1 console application with a graphical
interface, varied responses, conversation memory, follow-up
questions, and sentiment detection.

## Requirements

- Windows
- Visual Studio 2022 with support for .NET 8
- .NET desktop development workload
- .NET 8 SDK

## How to run

1. Download and extract the repository, or clone it using Git.
2. Open CybersecurityAwarenessBot.csproj in Visual Studio.
3. Allow Visual Studio to restore the project dependencies.
4. Select Build > Build Solution.
5. Press F5 to run the application.
6. Enter your name and click Start chat.
7. Type a message and press Enter or click Send.

Keep Media/greeting.wav in the project. The project copies this
file to the output folder when it builds.

## Features

- Windows Forms graphical interface.
- Original ASCII lock artwork.
- WAV voice greeting and Replay greeting button.
- Personalised greetings using the user's name.
- Recognition of eight cybersecurity topics.
- Random tips that avoid immediate repetition.
- Follow-up questions that keep the current topic.
- Memory of the user's name and favourite topic.
- Responses adapted to worried, frustrated, confused, and
  curious sentiments.
- Validation for empty messages and missing names.
- Helpful responses to unrecognised questions.
- New chat button that clears the conversation and memory.

## Supported topics

- Passwords
- Phishing
- Scams
- Privacy
- Safe browsing
- Suspicious links
- Malware
- Multi-factor authentication

## Example conversation

Send these messages separately:

1. Tell me about passwords
2. Another tip
3. Explain more
4. I'm interested in privacy
5. Tell me about scams
6. What do you remember?
7. I'm worried about scams

The bot should remember the privacy interest after changing topics
and provide both reassurance and advice when worry is detected.

## Project structure

| File | Purpose |
|---|---|
| Program.cs | Starts the Windows Forms application. |
| Models/User.cs | Stores user details and conversation context. |
| Models/Topic.cs | Defines the information stored for a topic. |
| Services/TopicCatalogue.cs | Stores topics, keywords, tips, and explanations. |
| Services/ChatbotService.cs | Handles messages, memory, follow-ups, and random tips. |
| Services/SentimentService.cs | Detects simple sentiments and supplies supportive wording. |
| Services/AudioService.cs | Loads and plays the WAV greeting. |
| UI/MainForm.cs | Creates the graphical interface and handles user interaction. |
| UI/ConsoleUI.cs | Preserves the original Part 1 console interface source. |
| Media/greeting.wav | Contains the voice greeting. |

## Programming techniques

The application uses classes and methods to separate the interface,
conversation logic, topic data, and audio handling.

A generic List<Topic> stores cybersecurity topics. Arrays store
keywords and tips. Dictionaries store sentiment keywords and the
previous tip selected for each topic.

A ResponseFormatter delegate, assigned a lambda expression,
personalises replies with the user's name.

Regular expressions match complete words and phrases. Random
selection provides varied tips while excluding the previous tip.

## Testing

The project built successfully in Visual Studio.

Manual testing checked:

- Name validation and name recall.
- All eight topics and topic buttons.
- Random replies and follow-up questions.
- Favourite-topic memory.
- Sentiment responses.
- Unknown and empty messages.
- Voice playback.
- New chat and closing the application.

## Limitations

Sentiment detection and topic recognition use predefined rules.
The bot does not understand every possible phrase.

Memory lasts only for the current chat. It resets when New chat
is confirmed or the application closes.

Voice playback requires the WAV file and working audio output.

## Troubleshooting

- If Windows Forms components are missing, install the .NET
  desktop development workload through Visual Studio Installer.
- If the greeting cannot play, check Media/greeting.wav and
  your audio output.
- If the application does not open after building, press F5.
- If a build fails, inspect Visual Studio's Error List.

## Video presentation

Video link: TO BE ADDED AFTER RECORDING

The presentation will demonstrate the running application and
explain its code structure, logic, collections, and delegate.
## Part 3 — Final POE

This project extends the existing Cybersecurity Awareness Chatbot.

### New Features
- Task Assistant with MySQL database
- Cybersecurity task reminders
- 12-question Cybersecurity Quiz
- Natural Language Processing simulation
- Activity Log
- Integration with Parts 1 and 2

### How to Run
1. Open the project in Visual Studio.
2. Ensure MySQL Server is running.
3. Configure the CYBERBOT_MYSQL connection.
4. Build and run the application.

### Video Presentation
Video link: To be added.
