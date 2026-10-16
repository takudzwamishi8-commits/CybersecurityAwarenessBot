using System.Collections.Generic;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    // Stores the keywords, tips and explanation for each topic.
    public static class TopicCatalogue
    {
        public static List<Topic> Create()
        {
            return new List<Topic>
            {
                new Topic(
                    "passwords",
                    new[]
                    {
                        "password", "passwords",
                        "passphrase", "passphrases"
                    },
                    new[]
                    {
                        "Use a long, unique password for every account. Avoid using your name or birthday.",

                        "Use a trusted password manager to generate and store strong passwords.",

                        "Enable multi-factor authentication. Never share your password, PIN or one-time password."
                    },
                    "A password is like a key to your account. " +
                    "Using a different password for each account stops " +
                    "one stolen password from unlocking all your accounts."
                ),

                new Topic(
                    "phishing",
                    new[]
                    {
                        "phishing", "phish",
                        "fake email", "suspicious email"
                    },
                    new[]
                    {
                        "Check the sender's full email address. A familiar name or logo does not prove that a message is genuine.",

                        "Avoid unexpected sign-in links. Open the organisation's official app or website yourself.",

                        "Be careful with messages demanding urgent action or offering unexpected prizes. Verify them through a trusted contact method."
                    },
                    "Phishing is a fake message pretending to come " +
                    "from someone you trust. For example, a fake bank " +
                    "email might lead to a copied website that steals your password."
                ),

                new Topic(
                    "scams",
                    new[]
                    {
                        "scam", "scams", "fraud",
                        "scammer", "scammers"
                    },
                    new[]
                    {
                        "Do not send money because of an urgent message. Call the person using a number you already trust.",

                        "Be careful with jobs, investments or prizes that require an upfront payment. Check the offer independently.",

                        "If you paid a suspected scammer, contact your bank immediately through its official channels and keep evidence."
                    },
                    "A scam tricks you into giving away money or " +
                    "information. Slow down, check the story independently, " +
                    "and never share your banking PIN or OTP."
                ),

                new Topic(
                    "privacy",
                    new[]
                    {
                        "privacy", "personal information",
                        "personal details", "oversharing"
                    },
                    new[]
                    {
                        "Review your social media privacy settings. Limit who can see your address, phone number and location.",

                        "Check app permissions. Only allow camera, microphone or location access when it is needed.",

                        "Share only the personal information a service needs. Check why a website wants your information before submitting it."
                    },
                    "Privacy means controlling who can see your " +
                    "personal information. For example, a private social " +
                    "profile helps limit who sees your photos."
                ),

                new Topic(
                    "safe browsing",
                    new[]
                    {
                        "safe browsing", "browsing",
                        "browser", "websites", "website"
                    },
                    new[]
                    {
                        "Keep your browser and operating system updated to receive security fixes.",

                        "Download files and apps from trusted official sources. Avoid pop-ups asking you to install unknown software.",

                        "Check website addresses carefully. HTTPS encrypts the connection, but it does not guarantee that a website is honest."
                    },
                    "Safe browsing means checking the websites you " +
                    "visit and the files you download. A fake shop can " +
                    "also use HTTPS, so check the seller and address."
                ),

                new Topic(
                    "suspicious links",
                    new[]
                    {
                        "suspicious link", "link",
                        "links", "url", "urls"
                    },
                    new[]
                    {
                        "Check a link's destination before opening it. On a computer, hovering over it can reveal the address.",

                        "Shortened links can hide their destination. If you are unsure, open the organisation's official app instead.",

                        "Do not enter your banking PIN or OTP after following an unexpected link. Verify the request through your bank."
                    },
                    "A link takes you to an address online. " +
                    "A button labelled 'My Bank' could actually " +
                    "send you to a scammer's website."
                ),

                new Topic(
                    "malware",
                    new[]
                    {
                        "malware", "virus",
                        "viruses", "ransomware"
                    },
                    new[]
                    {
                        "Install security updates and use reputable security software. Avoid unknown attachments.",

                        "Back up important files. Keep at least one backup disconnected from your device.",

                        "If you suspect an infection, stop entering sensitive information on that device and seek trusted technical help."
                    },
                    "Malware is harmful software. Ransomware is " +
                    "one type that locks your files and demands money. " +
                    "Updates, careful downloads and backups reduce the risk."
                ),

                new Topic(
                    "multi-factor authentication",
                    new[]
                    {
                        "mfa", "2fa", "two factor",
                        "two-factor", "multi-factor",
                        "authentication"
                    },
                    new[]
                    {
                        "Enable multi-factor authentication on important accounts. Use an authenticator app or security key when supported.",

                        "Never approve a sign-in request you did not make. Someone could be trying to access your account.",

                        "Keep recovery codes somewhere safe. Do not share them with anyone who contacts you."
                    },
                    "Multi-factor authentication adds another " +
                    "security check after your password, such as an " +
                    "authenticator app. This helps protect your account " +
                    "even if someone steals your password."
                )
            };
        }
    }
}