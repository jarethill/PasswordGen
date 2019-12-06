//TODO: 
//- Rewrite password memorization practice to be recursive
//- Organize methods into appropriate classes
//- Add option to use modifying operations when using multiple password generation
//- Add proper notes

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Linq;
using System.Diagnostics;
using static System.Console;

namespace PasswordGen
{
    public static class Program
    {
        public static void CreateXPasswords() // Refactor from menu choice 9. Generate multiple passwords & save to file
        {

        }
        
        public static void Main(string[] args)
        {
            SetWindowSize(62, 18);
            
            var diceDict = new Dictionary<string, string>(FillDictionary());
            var wordList = new List<string>();

            var execDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\";
            var saveFileName = "savedpasswords.txt";
            var fullFilePath = execDirectory + saveFileName;
            

            // Produces a word list to use for the following menu based on user choices
            wordList = InitialMenu(diceDict, wordList); 

            while (true)
            {
                PrintWordList(wordList);
                WriteLine("\nChoose an operation:\n\n" +
                          "1. Capitalize random letter\n" +
                          "2. Swap word order\n" +
                          "3. Convert some letters to leet speak (E.g. p4s5w0rd)\n" +
                          "4. Apply all modifying operations\n\n" +
                          "5. Copy password to clipboard\n" +
                          "6. Practice password memorization\n" +
                          "7. Save password to " + saveFileName + "\n" +
                          "8. Generate new password\n" +
                          "9. Generate multiple passwords & save to file\n" +
                          "0. Exit program\n");

                var operationChoice = StrToInt(ReadLine());
                var repeat = "Y";

                switch (operationChoice)
                {
                    case 1:
                        while (repeat != "N")
                        {
                            Clear();
                            var capitalizeBuffer = CapitalizeRandomLetter(wordList);
                            PrintWordList(capitalizeBuffer);

                            WriteLine("Repeat? [Y/N]\n");
                            repeat = ReadLine().ToUpper();

                            if (repeat == "N")
                            {
                                wordList.Clear();
                                foreach (var word in capitalizeBuffer)
                                    wordList.Add(word);
                            }
                        }
                        break;


                    case 2:
                        while (repeat != "N")
                        {
                            Clear();
                            var swapBuffer = new List<string>(wordList);
                            SwapWordOrder(swapBuffer);
                            PrintWordList(swapBuffer);

                            WriteLine("Repeat? [Y/N]\n");
                            repeat = ReadLine().ToUpper();

                            if (repeat == "N")
                            {
                                wordList.Clear();
                                foreach (var word in swapBuffer)
                                    wordList.Add(word);
                            }
                        }
                        break;

                    case 3:
                        while (repeat != "N")
                        {
                            Clear();
                            var leetBuffer = LeetSpeak(wordList);
                            PrintWordList(leetBuffer);

                            WriteLine("Repeat? [Y/N]\n");
                            repeat = ReadLine().ToUpper();

                            if (repeat == "N")
                            {
                                wordList.Clear();
                                foreach (var word in leetBuffer)
                                    wordList.Add(word);
                            }
                        }
                        break;

                    case 4:
                        while (repeat != "N")
                        {
                            var buffer0 = CapitalizeRandomLetter(wordList);
                            var buffer1 = SwapWordOrder(buffer0);
                            var buffer2 = LeetSpeak(buffer1);

                            PrintWordList(buffer2);

                            WriteLine("Repeat? [Y/N]\n");
                            repeat = ReadLine().ToUpper();

                            if (repeat == "N")
                            {
                                wordList.Clear();
                                foreach (var word in buffer2)
                                    wordList.Add(word);
                            }
                        }
                        break;

                    case 5:
                        var copy = ListToStr(wordList);
                        Clipboard.Copy(copy);
                        Clear();
                        Write("Word List copied to clipboard. Press any key to continue...");
                        ReadKey();
                        break;

                    case 6:
                        Clear();
                        var originalString = ListToStr(wordList);
                        var stringArray = originalString.Split(" ");
                        var finishedWord = new StringBuilder();

                        while (repeat != "N")
                        {
                            for (int i = stringArray.Length - 1; i >= 0; i--)
                            {
                                while (true)
                                {
                                    if (i == 0) // Prevents index out of bounds on final iteration
                                        break;

                                    var currentWord = stringArray[i - 1];
                                    var priorWord = "";

                                    if (i > 1)                           // 
                                        priorWord += stringArray[i - 2]; //
                                                                         // Prevents index out of bounds on final iteration
                                    else                                 //
                                        priorWord += stringArray[i - 1]; //

                                    if (i == (stringArray.Length - 1)) // Adds starting point for finishedWord, only done on first iteration
                                        finishedWord.Append(currentWord);

                                    WriteLine("Type: " + finishedWord.ToString());

                                    var input = ReadLine();
                                    while (input != finishedWord.ToString())
                                    {
                                        WriteLine("Try again.");
                                        input = ReadLine();
                                    }

                                    Clear();
                                    WriteLine("Current Progress: " + finishedWord.ToString());
                                    Thread.Sleep(600);
                                    Clear();

                                    finishedWord.Insert(0, priorWord + " ");

                                    break;
                                }
                            }

                            finishedWord.Remove(0, finishedWord.IndexOf(" ", 0, true)); // Removes extra word from finishedWord, hacky fix for my broken implementation. 
                            finishedWord.Remove(0, 1);

                            WriteLine("Type full password, or type !q to exit");
                            repeat = "Y";

                            while (repeat == "Y")
                            {
                                var fullPassword = ReadLine();

                                if (fullPassword == "!q")
                                {
                                    repeat = "N";
                                    break;
                                }

                                if (fullPassword == finishedWord.ToString())
                                {
                                    PrintWordList(wordList);
                                    WriteLine("\nSuccess!\n");
                                    Thread.Sleep(600);

                                    repeat = "N";
                                    finishedWord.Clear();
                                }

                                else
                                {
                                    WriteLine("Wrong answer, try again.");
                                }
                            }
                        }
                        break;

                        

                    case 7:
                        bool saveStatus = SavePassword(wordList, saveFileName, execDirectory);

                        if (saveStatus)
                            Write("File saved successfully.");

                        else
                            Write("File already contains this password.");

                        Thread.Sleep(600);
                        break;

                    case 8:
                        wordList.Clear();
                        wordList = new List<string>(InitialMenu(diceDict, wordList));
                        break;

                    case 9:
                        Clear();
                        WriteLine("How many passwords would you like to generate?");
                        var passwordsToGenerate = StrToInt(ReadLine());


                        Clear();
                        WriteLine("Which implementation would you like to use?\n\n" +
                         "1. Original Diceware algorithm\n" +
                         "2. Custom Diceware algorithm");

                        var implementationChoice = StrToInt(ReadLine());

                        while (implementationChoice < 1 || implementationChoice > 2)
                        {
                            implementationChoice = StrToInt(ReadLine());

                            if (implementationChoice < 1 || implementationChoice > 2)
                                WriteLine("Invalid input. Try again.");
                        }

                        Clear();
                        WriteLine("Choose length of your password, maximum of '10'");
                        var passwordLength = StrToInt(ReadLine());

                        while (passwordLength < 1 || passwordLength > 10)
                        {
                            WriteLine("Invalid length. Try again.");
                            passwordLength = StrToInt(ReadLine());
                        }

                        if (passwordsToGenerate <= 0)
                            WriteLine("Invalid input, returning to menu.");

                        else
                        {
                            var failCount = 0;
                            var wordListBackup = new List<string>(wordList);
                            var stopwatch = Stopwatch.StartNew();

                            Clear();
                            WriteLine("Please wait while passwords generate...");



                            for (int i = 0; i < passwordsToGenerate; i++)
                            {
                                wordList.Clear();
                                if (implementationChoice == 1)
                                    wordList = new List<string>(Diceware(diceDict, "ORIGINAL", passwordLength));

                                else
                                    wordList = new List<string>(Diceware(diceDict, "CUSTOM", passwordLength));


                                saveStatus = SavePassword(wordList, saveFileName, execDirectory);

                                if (saveStatus)
                                    continue;

                                else
                                    failCount++;

                            }

                            if (failCount > 0)
                            {
                                WriteLine(failCount + " passwords failed to save (already exist), returning to menu.");
                                Thread.Sleep(600);

                            }

                            else
                            {
                                var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

                                WriteLine("\nSave successful, total time to complete: " + elapsedSeconds + " seconds");
                                Write("Press any key to continue...");
                                ReadKey();

                            }

                            wordList.Clear();
                            wordList = new List<string>(wordListBackup);
                        }
                        break;
                    
                    case 0:
                        Write("Exiting program. Press any key to continue...");
                        ReadKey();
                        return;

                }
            }
        }
        public static List<string> InitialMenu(Dictionary<string, string> diceDict, List<string> wordList)
        {
            Clear();
            WriteLine("How would you like to generate your password?\n\n" +
                      "1. Choose your own words (e.g. word1, word2, word3 etc.)\n" +
                      "2. Choose words based off Diceware algorithm (Recommended)");

            var initialMenu = 0;
            var implementationChoice = 0;

            while (initialMenu < 1 || initialMenu > 2)
            {
                initialMenu = StrToInt(ReadLine());

                if (initialMenu < 1 || initialMenu > 2)
                    WriteLine("Invalid input. Try again.");
            }

            if (initialMenu == 2)
            {
                Clear();
                WriteLine("Which implementation would you like to use?\n\n" +
                          "1. Original Diceware algorithm\n" +
                          "2. Custom Diceware algorithm");

                implementationChoice = StrToInt(ReadLine());

                while (implementationChoice < 1 || implementationChoice > 2)
                {
                    implementationChoice = StrToInt(ReadLine());

                    if (implementationChoice < 1 || implementationChoice > 2)
                        WriteLine("Invalid input. Try again.");
                }
            }

            

            switch (initialMenu)
            {
                case 1:
                    wordList = new List<string>(OwnWords());
                    break;

                case 2:
                    Clear();
                    if (implementationChoice == 1)
                        wordList = new List<string>(Diceware(diceDict, "ORIGINAL"));

                    else
                        wordList = new List<string>(Diceware(diceDict, "CUSTOM"));
                        
                    break;
            }

            var outputList = new List<string>(wordList);
            return outputList;
        }
        public static bool SavePassword(List<string> wordList, string fileName, string fileDirectory)
        {
            var fullPath = fileDirectory + fileName;

            if (!File.Exists(fullPath))
                File.AppendAllText(fullPath, "");

            var passwordToSave = ListToStr(wordList);
            string allPasswords = File.ReadAllText(fullPath);

            if (!allPasswords.Contains(passwordToSave))
            {
                File.AppendAllText(fullPath, (passwordToSave + "\n"));
                return true;
            }

            else
                return false;
        }
        public static string OriginalRollDice()
        {
            var rnd = new Random();
            var finishedRoll = "";

            for (int i = 1; i <= 5; i++)
            {
                var roll = rnd.Next(1, 6);
                finishedRoll += roll;
            }

            return finishedRoll;
        }

        public static string CustomRollDice()
        {
            var rnd = new Random();
            var cryptoRnd = new RNGCryptoServiceProvider();
            var updatedDigits = new List<int>();

            while (true)
            {
                var byteArray = new byte[4];
                cryptoRnd.GetBytes(byteArray);
                var randomInteger = BitConverter.ToUInt32(byteArray, 0);
                var digits = randomInteger.ToString().Select(t => int.Parse(t.ToString())).ToArray();
                var count = 0;

                for (int i = 0; i < digits.Length; i++)
                {
                    if (digits[i] < 1 || digits[i] > 6)
                        continue;

                    updatedDigits.Add(digits[i]);
                    count++;
                }

                if (count < 5)
                {
                    count = 0;
                    updatedDigits.Clear();
                    continue;
                }

                else
                {
                    while (count > 5)
                    {
                        var lastIndex = (updatedDigits.Count - 1);
                        var finalWord = updatedDigits[lastIndex];

                        updatedDigits.Remove(finalWord);
                        count--;
                    }
                }

                break;
            }

            var output = "";

            for (int i = 0; i < updatedDigits.Count; i++)
            {
                output += updatedDigits[i];
            }

            return output;
        }
        public static Dictionary<string, string> FillDictionary()
        {
            var fileName = "dicewords.txt";
            var absolutePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var fullPath = absolutePath + @"\" + fileName;

            var diceDict = new Dictionary<string, string>();

            using (StreamReader sr = File.OpenText(fullPath))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    int position = line.IndexOf(",");
                    if (position < 0)
                        continue;

                    var key = line.Substring(0, position);
                    var value = line.Substring(position + 1);
                    diceDict.Add(key, value);
                }
            }

            return diceDict;
        }
        public static List<string> Diceware(Dictionary<string, string> myDict, string implementation = "original", int defaultGenerate = 0)
        {
            int wordCount = 0;
            if (defaultGenerate > 0)
            {
                wordCount = defaultGenerate;
            }

            else
            {
                WriteLine("Choose length of your password, maximum of '10'");
                wordCount = ChooseWordListLength();
            }
            
            var wordList = new List<string>();
            var count = 0;



            if (string.Equals(implementation, "ORIGINAL", StringComparison.OrdinalIgnoreCase))
            {
                while (count < wordCount)
                {
                    var roll = OriginalRollDice();
                    wordList.Add(myDict[roll]);
                    count++;
                }
            }

            else
            {
                while (count < wordCount)
                {
                    var roll = CustomRollDice();
                    wordList.Add(myDict[roll]);
                    count++;
                }
            }

            return wordList;
        }
        public static int ChooseWordListLength()
        {
            int wordCount;

            while (true)
            {
                var numOfWords = ReadLine();

                if (String.IsNullOrWhiteSpace(numOfWords))
                {
                    WriteLine("Invalid length. Try again.");
                    continue;
                }

                wordCount = StrToInt(numOfWords);

                while (wordCount < 1 || wordCount > 10)
                {
                    WriteLine("Invalid length. Try again.");
                    numOfWords = ReadLine(); wordCount = StrToInt(numOfWords);
                }

                break;
            }

            return wordCount;
        }
        public static IList<string> OwnWords()
        {
            WriteLine("Choose length of your password, maximum of '10'");
            int wordCount = ChooseWordListLength();

            Clear();
            WriteLine("Add words.");

            var wordList = new List<string>();

            for (int i = 0; i < wordCount; i++)
            {
                wordList.Add(ReadLine());
            }

            return wordList;
        }
        public static int StrToInt(string input)
        {
            int number;
            Int32.TryParse(input, out number);
            return number;
        }
        public static string ListToStr<T>(IList<T> input)
        {
            string output = "";

            for (int i = 0; i < input.Count; i++)
            {
                if (i != input.Count)
                    output += input[i] + " ";
                else // Prevents space being added to the final word in the list
                    output += input[i];
            }

            return output;
        }
        public static void PrintWordList(IList<string> input)
        {
            Clear();
            var output = ListToStr(input);
            WriteLine("Word List\n" + output);
        }
        public static IList<string> CapitalizeRandomLetter(IList<string> input)
        {
            List<string> newList = new List<string>();
            var rnd = new Random();

            for (int i = 0; i < input.Count; i++)
            {
                var currentWord = input[i];
                var randomNumber = rnd.Next(0, currentWord.Length);
                char randomLetter = currentWord[randomNumber];

                char[] ch = currentWord.ToCharArray();
                ch[randomNumber] = Char.ToUpper(randomLetter);
                var finishedWord = new string(ch);

                newList.Add(finishedWord);
            }

            return newList;
        }
        public static IList<string> LeetSpeak(IList<string> input)
        {
            List<string> newList = new List<string>();
            var rnd = new Random();

            for (int i = 0; i < input.Count; i++)
            {
                var currentWord = input[i];
                char[] ch = currentWord.ToCharArray();
                var finishedWord = "";

                for (int j = 0; j < ch.Length; j++)
                {
                    var chance = rnd.Next(1, 4);
                    var letter = ch[j];

                    if (chance == 1)
                    {
                        if (letter == 'a' || letter == 'A')
                            letter = '4';

                        if (letter == 'e' || letter == 'E')
                            letter = '3';

                        if (letter == 't' || letter == 'T')
                            letter = '7';

                        if (letter == 'o' || letter == 'O')
                            letter = '0';

                        if (letter == 's' || letter == 'S')
                            letter = '5';

                        if (letter == 'B')
                            letter = '8';

                        if (letter == 'G')
                            letter = '6';
                    }

                    finishedWord += letter;
                }

                newList.Add(finishedWord);
            }

            return newList;
        }
        public static IList<T> SwapWordOrder<T>(this IList<T> list)
        {
            Random random = new Random();

            for (int i = list.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);

                T value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }

            return list;
        }

        public static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase) // Extension method to add IndexOf to StringBuilder. Needed to fix an error in password memorization practice
        {
            int index;
            int length = value.Length;
            int maxSearchLength = (sb.Length - length) + 1;

            if (ignoreCase)
            {
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (Char.ToLower(sb[i]) == Char.ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (Char.ToLower(sb[i + index]) == Char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }

                return -1;
            }

            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }


    }
}