using System;
using System.Text;
using Sys = Cosmos.System;
using System.Threading;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using System.IO;
using Cosmos.System.Graphics;
using CommanderLibraries_v2;
using CGUI;
using LightningOS.LSSystem.Apps;

namespace LightningOS
{
    public class Kernel : Sys.Kernel
    {
      

        static ConsoleColor current_foreground = ConsoleColor.White;
        static ConsoleColor current_background = ConsoleColor.Black;
        static string currentForegroundString = "white";
        static string currentBackgroundString = "black";
        static string currentUserName = "root";
        static bool isOutOfBoot = true;
        string versionString = "1.0 Stable";
        public string current_directory = @"0:\";
        Canvas canvas;


        public static void saveDisk()
        {
            Console.Clear();
            Console.Beep();
            Console.Clear();
            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("LightningOS SaveDisk Utility");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(x, y);

            Console.WriteLine("A fatal error has occured with the filesystem.\nCause: Bad / Failed setup on boot partition." +
                    "\nFilesystem state: Crashed\n");

            Thread.Sleep(500);
            

            print("\n1. Reset filesystem");
            print("2. Attempt a Reboot");
            print("3. Quit");
            Console.Write("Enter choice> ");
            string choice = Console.ReadLine();
            if (choice == "1")
            {
                Console.WriteLine("Proceeding with this operation will re-format your filesystem.\nAll the data will be lost.");
                Console.Write("Do you want to continue [Y/N]> ");
                string yn = Console.ReadLine();
                if (yn.ToLower() == "y")
                {
                    Directory.CreateDirectory("0:\\TEST");
                    Console.WriteLine("Setup initiated. Rebooting the system....");
                    Sys.Power.Reboot();
                }
                else
                {
                    Console.Clear();
                }
            }
            else
            {
                Console.Clear();
            }
        }

        public static void filesystem_settingsConfig()
        {
            Console.Clear();
            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("LightningOS System Configuration - Filesystem");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.ForegroundColor = current_foreground;
            Console.BackgroundColor = current_background;
            Console.SetCursorPosition(x, y);

            print("\n1. Change startup directory");
            print("2. Back");

            Console.Write("Enter Choice> ");
            string choice = Console.ReadLine();

            if(choice == "1")
            {
                string defaultPath = "";
                Console.Write("Enter path to boot to> ");
                defaultPath = Console.ReadLine();
                if (Directory.Exists(defaultPath))
                {
                    File.WriteAllText("0:\\System\\LightningOS\\Prefs\\defpth.prefs", defaultPath);
                } else
                {
                    Console.WriteLine("Directory does not exist.");
                }
                handleSysPrefs();
                
            } else if(choice == "2")
            {
                handleSysPrefs();
            } else
            {
                handleSysPrefs();
            }
        }

        public static void themes_settingsConfig()
        {
            Console.Clear();
            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("LightningOS System Configuration - Theme");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.ForegroundColor = current_foreground;
            Console.BackgroundColor = current_background;
            Console.SetCursorPosition(x, y);

            print("\n1. Set theme to default");
            print("2. Set theme to black on white");
            print("3. Set theme to green on black");
            print("4. Set theme to custom");
            print("5. Back");

            Console.Write("Enter Choice> ");
            string choice = Console.ReadLine();
            if(choice == "1")
            {
                
                current_foreground = ConsoleColor.White;
                current_background = ConsoleColor.Black;
                
               
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrfrgrd.prefs", "white");
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrbkgrd.prefs", "black");
                Console.WriteLine("Please reboot to activate changes.");
                handleSysPrefs();
            } else if(choice == "2")
            {
                current_foreground = ConsoleColor.Black;
                current_background = ConsoleColor.White;
                
               
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrfrgrd.prefs", "black");
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrbkgrd.prefs", "white");
                Console.WriteLine("Please reboot to activate changes.");
                handleSysPrefs();
            } else if(choice == "3")
            {
                current_foreground = ConsoleColor.Green;
                current_background = ConsoleColor.Black;
                
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrfrgrd.prefs", "green");
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrbkgrd.prefs", "black");
                Console.WriteLine("Please reboot to activate changes.");
                
                handleSysPrefs();
            } else if(choice == "4")
            {
                Console.Write("Enter foreground color> ");
                string foreground = Console.ReadLine().ToLower();
                if(foreground == "green")
                {
                    current_foreground = ConsoleColor.Green;
                    currentForegroundString = "green";
                } else if(foreground == "yellow")
                {
                    currentForegroundString = "yellow";
                    current_foreground = ConsoleColor.Yellow;
                } else if(foreground == "pink")
                {
                    current_foreground = ConsoleColor.Magenta;
                    currentForegroundString = "pink";
                } else if(foreground == "red")
                {
                    current_foreground = ConsoleColor.Red;
                    currentForegroundString = "red";
                } else if(foreground == "blue")
                {
                    currentForegroundString = "blue";
                    current_foreground = ConsoleColor.Blue;
                } else if(foreground == "orange")
                {
                    currentForegroundString = "orange";
                    current_foreground = ConsoleColor.DarkYellow;
                } else if(foreground == "white")
                {
                    currentForegroundString = "white";
                    current_foreground = ConsoleColor.White;
                } else if(foreground == "black")
                {
                    currentForegroundString = "black";
                    current_foreground = ConsoleColor.Black;
                } else
                {
                    Console.WriteLine("Color not avaliable.");
                    themes_settingsConfig();
                }

                Console.Write("Enter background color> ");
                string background = Console.ReadLine().ToLower();
                if (background == "green")
                {
                    currentBackgroundString = "green";
                    current_background = ConsoleColor.Green;
                }
                else if (background == "yellow")
                {
                    currentBackgroundString = "yellow";
                    current_background = ConsoleColor.Yellow;
                }
                else if (background == "pink")
                {
                    currentBackgroundString = "pink";
                    current_background = ConsoleColor.Magenta;
                }
                else if (background == "red")
                {
                    currentBackgroundString = "red";
                    current_background = ConsoleColor.Red;
                }
                else if (background == "blue")
                {
                    currentBackgroundString = "blue";
                    current_background = ConsoleColor.Blue;
                }
                else if (background == "orange")
                {
                    currentBackgroundString = "orange";
                    current_background = ConsoleColor.DarkYellow;
                }
                else if (background == "white")
                {
                    currentBackgroundString = "white";
                    current_background = ConsoleColor.White;
                }
                else if (background == "black")
                {
                    currentBackgroundString = "black";
                    current_background = ConsoleColor.Black;
                }
                else
                {
                    Console.WriteLine("Color not avaliable.");
                    themes_settingsConfig();
                }

                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrfrgrd.prefs", currentForegroundString);
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrbkgrd.prefs", currentBackgroundString);
                Console.WriteLine("Please reboot to activate changes.");
                handleSysPrefs();
            }
            else if (choice == "5")
            {
                handleSysPrefs();
            }
            else
            {
                handleSysPrefs();
            }
        }

        public static void disks_os_settingsConfig()
        {
            Console.Clear();
            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("LightningOS System Configuration - Disks\\OS");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.ForegroundColor = current_foreground;
            Console.BackgroundColor = current_background;
            Console.SetCursorPosition(x, y);

            print("\n1. Reboot");
            print("2. Back");

            Console.Write("Enter Choice> ");
            string choice = Console.ReadLine();

            if(choice == "1")
            {
                Sys.Power.Reboot();
            } else if(choice == "2")
            {
                handleSysPrefs();
            } else
            {
                handleSysPrefs();
            }
        }

        public static void users_password_settingsConfig()
        {
            Console.Clear();
            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("LightningOS System Configuration - Users\\Passwords");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.ForegroundColor = current_foreground;
            Console.BackgroundColor = current_background;
            Console.SetCursorPosition(x, y);

            print("\n1. Change @" + currentUserName + " Password");
            print("2. Change Username");
            print("3. Back");

            Console.Write("Enter Choice> ");
            string choice = Console.ReadLine();
            if(choice == "1")
            {
                string rightPassword = File.ReadAllText("0:\\System\\LightningOS\\Prefs\\usrconfig.prefs");
                Console.Write("Enter current password> ");
                string enteredPassword = TextFormatting.PasswordMask('*');
                if (enteredPassword == rightPassword)
                {
                    string newPassword = "";
                    Console.Write("Enter new password> ");
                    newPassword = TextFormatting.PasswordMask('*');
                    File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrconfig.prefs", newPassword);
                    handleSysPrefs();
                }else
                {
                    handleSysPrefs();
                }
            } else if(choice == "2")
            {
                string newName = "";
                Console.Write("Enter new username> ");
                newName = Console.ReadLine();
                currentUserName = newName;
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrrt.prefs", newName);
                handleSysPrefs();
            } else if(choice == "3")
            {
                handleSysPrefs();
            } else
            {
                Console.Clear();
            }
            
        }

        public static void handleSysPrefs()
        {
            Console.Clear();

            //Print top bar.

            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("LightningOS System Configuration" + "                                 " + "\n");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.ForegroundColor = current_foreground;
            Console.BackgroundColor = current_background;
            Console.SetCursorPosition(x, y);

            print("\n1. Users\\Passwords");
            print("2. Disks\\OS");
            print("3. FileSystem");
            print("4. Theme (Look and Feel)");
            print("5. Exit");

            Console.Write("Enter choice> ");
            string choice = Console.ReadLine();

            if(choice == "1")
            {
                users_password_settingsConfig();

            } else if(choice == "2")
            {
                disks_os_settingsConfig();

            } else if(choice == "3")
            {
                filesystem_settingsConfig();

            } else if(choice == "4")
            {
                themes_settingsConfig();

            } else if(choice == "5")
            {
                Console.Clear();
            }
        }

        public static void print(string text)
        {
            Console.WriteLine(text);
            
        }

        public static void loginManager()
        {
            Console.Write("Password for @" + currentUserName + "> ");
            string password = TextFormatting.PasswordMask('*');
            string filePassword = File.ReadAllText("0:\\System\\LightningOS\\Prefs\\usrconfig.prefs");
            if(password == filePassword)
            {
                
            }
            else
            {
                Console.Write("Password for @" + currentUserName + "> ");
                string password2 = TextFormatting.PasswordMask('*');
                if (password2 == filePassword)
                {

                }
                else
                {
                    Console.Write("Password for @" + currentUserName + "> ");
                    string password3 = TextFormatting.PasswordMask('*');
                    if (password3 == filePassword)
                    {

                    }
                    else
                    {
                        print("Incorrect password. Shuting down....");
                        Sys.Power.Shutdown();

                    }

                }
            }
        }

        public static void findSystemPrefs()
        {
            //Find them.

        }

        public static void createFileSystem()
        {
            CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
            try
            {
                
                VFSManager.RegisterVFS(fs);
                VFSManager.SetFileSystemLabel("0:\\", "LocalDisk (0:\\)");
            }
            catch
            {
                Console.WriteLine("Fatal OS Error. Shutting down....");
                Sys.Power.Shutdown();

            }
            

        }
        public static void printLightning()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Lightning");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("OS ");
           
        }
        protected override void BeforeRun()
        {
            //Boot up. Configure filesystem, and display 'Welecome' message.

            var splashImageAscii = @"
.^^^^^^^^^^^^^^^^^~:                                                                                                                                                                                                                                                                                                                                      
                                                     .~~~~~~~~~~~~~~~~~~^.                                                                                                                                                                                                                                                                                                                                      
                                                    .~~~~~~~~~~~~~~~~~~~.                                                                                                                                                                                                                                                                                                                                       
                                                   .~~~~~~~~~~~~~~~~~~~:                                                                                                                                                                                                                                                                                                                                        
                                                  .~~~~~~~~~~~~~~~~~~~^                                                                                                                                                                                                                                                                                                                                         
                                                 .~~~~~~~~~~~~~~~~~~~~.                                                                                                                                                                                                                                                                                                                                         
                                                .~~~~~~~~~~~~~~~~~~~~:                                                                                                                                                                                                                                                                                                                                          
                                               .~~~~~~~~~~~~~~~~~~~~^                                                                                                                                                                                                                                                                                                                                           
                                              .^~~~~~~~~~~~~~~~~~~~^.                                                                                                                                                                                                                                                                                                                                           
                                             .^~~~~~~~~~~~~~~~~~~~~:            .                                                                                                                                                                                                                                                                                                                               
                                            .^~~~~~~~~~~~~~~~~~~~~~^^~~~~~~~~^~^:                                                                                                                                                                                                                                                                                                                               
                                           .~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~^.        ::     ~~          !?                      ~^                     :~~~:     ^~~:                                                                                                                                                                                                                                        
                                          .~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~^.          5#.    ??    ..    5#. .    ?Y       .     ?7     .       .     !GY??JPP^ :G57?Y~                                                                                                                                                                                                                                       
                                         .^~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~:            P#.    JY  75JJP5~ Y#JJYP~ ?B#J? .PYJJ55:  YJ ~G?JYP?  :YYJ5GJ.~@7     P#.~&Y^                                                                                                                                                                                                                                          
                                        .^~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~^.             P#.    GG .#Y .BG  Y&^  5#. 5G   .#P. :&Y  BP !@7  7@~ ?&^ 7@~ J@:     J@^ :?5P5~                                                                                                                                                                                                                                       
                                       .^~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~^.               P#.    PG .BPJYJ:  Y#.  J&. 5G   .#Y  .B5  BP !@~  ~@! ?#YYY!  ~@J    :BP .   .G&.                                                                                                                                                                                                                                      
                                      .~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~:                 Y#YYYJ.5P :#5J?J5^ JG.  ?G. 7BY? .GJ  .GY  PY ~#^  ^#~ JBJJ?Y?  ~P5YYYP?. !5YJYP!                                                                                                                                                                                                                                       
                                      ::::::::::::::^~~~~~~~~~~~~~~~^.                   .....  . !&7~~?#7  .    .   .:.  .    .   .   .    .  GP~~~PG.   .::.     .::.                                                                                                                                                                                                                                         
                                                   :~~~~~~~~~~~~~~^.                               ^!7!!:                                      .~!7!^                                                                                                                                                                                                                                                           
                                                  .~~~~~~~~~~~~~~:                                                                                                                                                                                                                                                                                                                                              
                                                 .~~~~~~~~~~~~~:.                                                                                                                                                                                                                                                                                                                                               
                                                .^~~~~~~~~~~~^.                                                                                                                                                                                                                                                                                                                                                 
                                                ^~~~~~~~~~~^:                                                                                                                                                                                                                                                                                                                                                   
                                               :~~~~~~~~~~^.                                                                                                                                                                                                                                                                                                                                                    
                                              .~~~~~~~~~^.                                                                                                                                                                                                                                                                                                                                                      
                                             .^~~~~~~~~:                                                                                                                                                                                                                                                                                                                                                        
                                            .^~~~~~~~:.                                                                                                                                                                                                                                                                                                                                                         
                                            ^~~~~~~^.                                                                                                                                                                                                                                                                                                                                                           
                                           :~~~~~^:                                                                                                                                                                                                                                                                                                                                                             
                                          :~~~~~^.                                                                                                                                                                                                                                                                                                                                                              
                                         .~~~~^:                                                                                                                                                                                                                                                                                                                                                                
                                        .~~~~:                                                                                                                                                                                                                                                                                                                                                                  
                                       .^~~^.                                                                                                                                                                                                                                                                                                                                                                   
                                       ^~^.                                                                                                                                                                                                                                                                                                                                                                     
                                      :~:                                                                                                                                                                                                                                                                                                                                                                       
                                     .^.                                
";

            var lightningBolt = @"
          d$$$$$P
         d$$$$$P
        $$$$$$""
      .$$$$$$""
     .$$$$$$""
    4$$$$$$$$$$$$$""
   z$$$$$$$$$$$$$""
   """"""""""""""3$$$$$""
         z$$$$P
        d$$$$""
      .$$$$$""
     z$$$$$""
    z$$$$P
   d$$$$$$$$$$""
  *******$$$""
       .$$$""
      .$$""
     4$P""
    z$""
   zP
  z""
";

            var title = @"
+-+-+-+-+-+-+-+-+-+-+-+
|L i g h t n i n g OS|
+-+-+-+-+-+-+-+-+-+-+-+
";



            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            //printLightning();
            //Console.WriteLine("Booting up....");
            Thread.Sleep(1000);
            //printLightning();
            Console.WriteLine("Kernel Started");
            Thread.Sleep(1000);
            createFileSystem();
            //printLightning();
            //Console.WriteLine("FileSystem (FAT32) Initialized");
            Thread.Sleep(1000);
            //printLightning();
            //Console.WriteLine("Finding SysPrefs...");
            findSystemPrefs();
            Thread.Sleep(1000);
            //Init
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(lightningBolt);
            Console.Write("\t\t" + title);
            Thread.Sleep(2000);
            Console.Clear();
            printLightning();
            Console.WriteLine("Version " + versionString);
            Console.WriteLine("Welecome!\n");

            

            

            if (Directory.Exists("0:\\TEST"))
            {
                Console.Clear();
                Console.Beep();
                Console.Write("Welecome to ");
                printLightning();
                Console.Write("Setup\n");
                Console.WriteLine("\nSetting up for the first time....\n");
                try
                {
                    File.Delete("0:\\TEST\\DirInTest\\Readme.txt");
                    Console.WriteLine("Building Filesystem -> 0:\\System");
                    Directory.CreateDirectory("0:\\System");

                }
                catch (Exception ex)
                {

                }
                try
                {
                    File.Delete("0:\\Kudzu.txt");
                    Console.WriteLine("Building Filesystem -> 0:\\System\\LightningOS");
                    Directory.CreateDirectory("0:\\System\\LightningOS");

                }
                catch (Exception ex)
                {

                }

                try
                {
                    File.Delete("0:\\Root.txt");
                    Console.WriteLine("Building Filesystem -> 0:\\System\\LightningOS\\Core");

                }
                catch (Exception ex)
                {

                }
                try
                {
                    Directory.Delete("0:\\TEST\\DirInTest");
                    Console.WriteLine("Building Filesystem -> 0:\\System\\LightningOS\\bin");
                    Directory.Delete("0:\\Dir Testing");
                    Console.WriteLine("Building Filesystem -> 0:\\Apps");

                }
                catch (Exception ex)
                {

                }
                try
                {
                    Directory.Delete("0:\\TEST");
                    Console.WriteLine("Building Filesystem -> Writing System Files....");
                    //Setup system directories.

                    
                    
                    Directory.CreateDirectory("0:\\System\\LightningOS\\Prefs");
                    string prefsText =
                        "\nstatic: noPrefs";
                    File.WriteAllText("0:\\System\\LightningOS\\Prefs\\prefs.pfs", prefsText);
                    Directory.CreateDirectory("0:\\Help");
                    Directory.CreateDirectory("0:\\Apps");
                    Directory.CreateDirectory("0:\\Apps\\Edit");
                    string editTextExecutable =
                        "\n//Edit (c) PIRUX Team\nstdlib.load(edit.lght)\nwhile \"edit.light\" isLooping:\neditClass.init()\neditClass.read()\neditClass.loop()\nwhileEnd";
                    File.WriteAllText("0:\\Apps\\Edit\\edit_init.config", editTextExecutable);



                    //Setup helpFile
                    string helpText =
                        "Welecome to LightningOS Docs!\nCommands:\n|\n|> FileSystem Commands\nfstype - Determine the filesystem type\ndir - List contents of current directory\ncd - Change current directory\nmkdir - Create a directory at given path" +
                        "\nrmdir - Delete directory at given path\n del - Delete file at given path\n read - Display the contents of a file at a given path\ntouch - Create a new file at given path\n\n|\n|> System Commands\nshutdown - Shutdown the computer. Use -now to shutdown immediantly.\ncls - Clear the screen" +
                        "\npwd - Show current working directory" +
                        "\nedit - Open a file at a given path in a text editor. Use -new to create new\nsysinf - Display system information" +
                        "\nsysconfig - Access system settings\ndrive - Display drive contents, and detailed info.\ncp - Copy a file to given path\nmv - Move a file to given path";
                    File.WriteAllText("0:\\Help\\Help.txt", helpText);

                }
                catch (Exception ex)
                {

                }

                


                string readmeText = "Welecome to LightningOS! Please refer to 0:\\Help\\Help.txt for learning commands and getting started.";
                File.WriteAllText("0:\\Readme.txt", readmeText);
                string password = "root";
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrconfig.prefs", password);

                //Boot config files
                Directory.CreateDirectory("0:\\System\\LightningOS\\Core");
                Console.WriteLine("Writing Help files....");
                Directory.CreateDirectory("0:\\System\\LightningOS\\Core\\bin");
                //.bin files: dir, fscmd
                Console.WriteLine("Writing 0:\\System\\LightningOS\\Core\\bin\\dir.bin to file....");
                File.WriteAllText("0:\\System\\LightningOS\\Core\\bin\\dir.bin", Utilities.FirstRunFileContents.mkdirCommandBin);
                Console.WriteLine("Writing 0:\\System\\LightningOS\\Core\\bin\\fscmd.bin to file....");
                File.WriteAllText("0:\\System\\LightningOS\\Core\\bin\\fscmd.bin", Utilities.FirstRunFileContents.copyCommandSource);

                Console.WriteLine("Cleaning up temp files....");



            }
            try
            {
                string currentDir = File.ReadAllText("0:\\System\\LightningOS\\Prefs\\defpth.prefs");
                if (currentDir == "")
                {

                }
                else
                {
                    current_directory = currentDir;
                }
            } catch
            {
                Console.WriteLine("Creating your preferences....");
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\defpth.prefs", "0:\\");
            }
            
           


            //File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrfrgrd.prefs", current_foreground.ToString());
            //File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrbkgrd.prefs", current_background.ToString());






            //SysBoot.SystemBoot();




            try
            {
                currentUserName = File.ReadAllText("0:\\System\\LightningOS\\Prefs\\usrrt.prefs");
            } catch (Exception ex)
            {
                Console.WriteLine("Setting up root user.... -> Password is \"root\"");
                File.WriteAllText("0:\\System\\LightningOS\\Prefs\\usrrt.prefs", "root");
                currentUserName = "root";
            }

            try
            {
                loginManager();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.Clear();
            printLightning();
            Console.WriteLine(" Version " + versionString);
            Console.WriteLine("Welecome!\n");





        }

        public static void printArrow()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(" -> ");
            Console.ForegroundColor = current_foreground;
        }

        public static void printRootText(string currentDir)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("@" + currentUserName + " ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(currentDir);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(">");
            Console.ForegroundColor = current_foreground;
            Console.BackgroundColor = current_background;
        }

        protected override void Run()
        {

            try
            {
                string readForeground = File.ReadAllText("0:\\System\\LightningOS\\Prefs\\usrfrgrd.prefs");
                string readBackground = File.ReadAllText("0:\\System\\LightningOS\\Prefs\\usrbkgrd.prefs");

                if (readForeground == "green")
                {
                    current_foreground = ConsoleColor.Green;
                }
                else if (readForeground == "yellow")
                {
                    current_foreground = ConsoleColor.Yellow;
                }
                else if (readForeground == "pink")
                {
                    current_foreground = ConsoleColor.Magenta;
                }
                else if (readForeground == "red")
                {
                    current_foreground = ConsoleColor.Red;
                }
                else if (readForeground == "blue")
                {
                    current_foreground = ConsoleColor.Blue;
                }
                else if (readForeground == "orange")
                {
                    current_foreground = ConsoleColor.DarkYellow;
                }
                else if (readForeground == "white")
                {
                    current_foreground = ConsoleColor.White;
                }
                else if (readForeground == "black")
                {
                    current_foreground = ConsoleColor.Black;
                }


                if (readBackground == "green")
                {
                    current_background = ConsoleColor.Green;
                }
                else if (readBackground == "yellow")
                {
                    current_background = ConsoleColor.Yellow;
                }
                else if (readBackground == "pink")
                {
                    current_background = ConsoleColor.Magenta;
                }
                else if (readBackground == "red")
                {
                    current_background = ConsoleColor.Red;
                }
                else if (readBackground == "blue")
                {
                    current_background = ConsoleColor.Blue;
                }
                else if (readBackground == "orange")
                {
                    current_background = ConsoleColor.DarkYellow;
                }
                else if (readBackground == "white")
                {
                    current_background = ConsoleColor.White;
                }
                else if (readBackground == "black")
                {
                    current_background = ConsoleColor.Black;
                }
            }
            catch
            {

            }



            if (Directory.Exists("0:\\TEST") || !Directory.Exists("0:\\System"))
            {
                saveDisk();
            }

            if (isOutOfBoot != false)
            {
                //Obtain Input
                printRootText(current_directory);
                //@root 0:\>
                string input = Console.ReadLine();
                input = input.ToLower();

                ///
                ///System health commands
                ///

                if(input == "svdsk")
                {
                    saveDisk();
                }

                ///
                /// End System health commands
                ///

                ///
                ///Filesystem commands
                ///

                if (input == "fstype")
                {
                    string fs_type = VFSManager.GetFileSystemType("0:\\");
                    Console.WriteLine("File System Type: " + fs_type);
                }

                if (input == "dir")
                {
                    try
                    {
                        var directory_list = VFSManager.GetDirectoryListing(current_directory);
                        if (directory_list.Count == 0)
                        {
                            Console.WriteLine("Directory is empty.");
                        }
                        else
                        {
                            foreach (var directoryEntry in directory_list)
                            {
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write("|>");
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(directoryEntry.mName);
                                Console.Write(" | Size: " + directoryEntry.GetUsedSpace() + " bytes");
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write("\n|\n");
                                Console.ForegroundColor = ConsoleColor.White;

                                
                                //|>dir
                                //|
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error locating directories.");
                        Console.Write("Show Stack? (Y/N)");
                        string result = Console.ReadLine().ToString();
                        if (result.ToString().ToLower() == "y")
                        {
                            Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                            Console.WriteLine("\n" + ex.ToString());
                        }
                    }


                }

                if (input == "cd")
                {
                    Console.Write("Enter directory path>");
                    string newDir = Console.ReadLine();

                    try
                    {
                        if (newDir == "")
                        {
                            Directory.SetCurrentDirectory(@"0:\");
                            current_directory = Directory.GetCurrentDirectory();
                        }
                        else
                        {
                            if (Directory.Exists(newDir))
                            {
                                Directory.SetCurrentDirectory(newDir);
                                current_directory = Directory.GetCurrentDirectory();
                            }
                            else
                            {
                                Console.WriteLine("The directory does not exist.");

                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error switching to directory " + newDir + "." + " The directory probably dosen't exist.");
                    }

                }

                if (input == "mkdir")
                {
                    Console.Write("Make directory at path> ");

                    string dirname = Console.ReadLine();
                    try
                    {
                        Directory.CreateDirectory(dirname);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error creating directory " + dirname);
                        Console.Write("Show Stack? (Y/N)");
                        string result = Console.ReadLine().ToString();
                        if (result.ToString().ToLower() == "y")
                        {
                            Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                            Console.WriteLine("\n" + ex.ToString());
                        }
                    }

                }

                if (input == "rmdir")
                {
                    Console.Write("Delete directory at path>");
                    string dirname = Console.ReadLine();
                    try
                    {
                        Directory.Delete(dirname);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error deleting directory " + dirname);
                        Console.Write("Show Stack? (Y/N)");
                        string result = Console.ReadLine().ToString();
                        if (result.ToString().ToLower() == "y")
                        {
                            Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                            Console.WriteLine("\n" + ex.ToString());
                        }
                    }

                }

                if (input == "del")
                {
                    Console.Write("Delete file at path>");
                    string filename = Console.ReadLine();
                    try
                    {
                        File.Delete(filename);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error deleting file " + filename);
                        Console.Write("Show Stack? (Y/N)");
                        string result = Console.ReadLine().ToString();
                        if (result.ToString().ToLower() == "y")
                        {
                            Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                            Console.WriteLine("\n" + ex.ToString());
                        }
                    }

                }

                if (input == "read")
                {
                    Console.Write("Read file at path>");
                    string filename = Console.ReadLine();
                    //Console.WriteLine("File: " + filename);
                    if (File.Exists(filename))
                    {
                        try
                        {
                            try
                            {
                                var user_file = VFSManager.GetFile(filename);
                                var user_file_stream = user_file.GetFileStream();

                                if (user_file_stream.CanRead)
                                {

                                    Console.WriteLine("\n" + filename + " (read)==============================>" + "\n");

                                    try
                                    {
                                        byte[] text_to_read = new byte[user_file_stream.Length];
                                        user_file_stream.Read(text_to_read, 0, (int)user_file_stream.Length);
                                        Console.WriteLine(Encoding.Default.GetString(text_to_read));
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Error reading file " + filename);
                                        Console.Write("Show Stack? (Y/N)");
                                        string result = Console.ReadLine().ToString();
                                        if (result.ToString().ToLower() == "y")
                                        {
                                            Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                                            Console.WriteLine("\n" + ex.ToString());
                                        }
                                    }
                                    Console.WriteLine();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error reading file " + filename);
                                Console.Write("Show Stack? (Y/N)");
                                string result = Console.ReadLine().ToString();
                                if (result.ToString().ToLower() == "y")
                                {
                                    Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                                    Console.WriteLine("\n" + ex.ToString());
                                }
                            }



                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error reading file " + filename);
                            Console.Write("Show Stack? (Y/N)");
                            string result = Console.ReadLine().ToString();
                            if (result.ToString().ToLower() == "y")
                            {
                                Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                                Console.WriteLine("\n" + ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("File does not exist/is not valid.");
                    }




                }

                if (input == "touch")
                {
                    Console.Write("Create file at path>");
                    string newFileName = Console.ReadLine();
                    try
                    {
                        VFSManager.CreateFile(newFileName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error creating file " + newFileName);
                        Console.Write("Show Stack? (Y/N)");
                        string result = Console.ReadLine().ToString();
                        if (result.ToString().ToLower() == "y")
                        {
                            Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                            Console.WriteLine("\n" + ex.ToString());
                        }
                    }
                }

                ///
                ///End filesystem commands
                ///

                ///
                ///System Commands
                ///

                if(input == "cmd")
                {
                    CMD.CMDRun();
                }

                if (input == "shutdown -now")
                {                 
                    Sys.Power.Shutdown();
                }

                if (input == "cls")
                {
                    Console.Clear();
                }

                if (input == "edit")
                {
                    //Console.Write("Edit file at path>");
                    //string newFileName = Console.ReadLine();
                    //if (File.Exists(newFileName))
                    //{
                    //    //LiquidEditor.Start(newFileName, current_directory);
                    //    MIV.MIV.StartMIV();
                    //}
                    //else
                    //{
                    //    Console.WriteLine("File does not exist/is not valid.");
                    //}

                    MIV.MIV.StartMIV();




                }

                if (input == "edit -new")
                {

                    LiquidEditor.Start(current_directory);

                }

                if(input == "cp")
                {
                    string copy_from;
                    string copy_to;
                    Console.Write("Enter path of file>");
                    copy_from = Console.ReadLine();
                    Console.Write("Enter path to copy to>");
                    copy_to = Console.ReadLine();

                    try
                    {
                        File.Copy(copy_from, copy_to);
                    } catch (Exception ex)
                    {
                        Console.WriteLine("Error copying file " + copy_from);
                        Console.Write("Show Stack? (Y/N)");
                        string result = Console.ReadLine().ToString();
                        if (result.ToString().ToLower() == "y")
                        {
                            Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                            Console.WriteLine("\n" + ex.ToString());
                        }
                    }


                    
                }

                if(input == "mv")
                {
                    try
                    {
                        string move_from;
                        string move_to;
                        Console.Write("Enter path of file>");
                        move_from = Console.ReadLine();
                        Console.Write("Enter path to move to>");
                        move_to = Console.ReadLine();

                        try
                        {
                            File.Copy(move_from, move_to);
                            File.Delete(move_from);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error moving file " + move_from);
                            Console.Write("Show Stack? (Y/N)");
                            string result = Console.ReadLine().ToString();
                            if (result.ToString().ToLower() == "y")
                            {
                                Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                                Console.WriteLine("\n" + ex.ToString());
                            }
                        }
                    } catch(Exception ex)
                    {
                        Console.WriteLine("Error moving file");
                        Console.Write("Show Stack? (Y/N)");
                        string result = Console.ReadLine().ToString();
                        if (result.ToString().ToLower() == "y")
                        {
                            Console.WriteLine("Stacktrace " + DateTime.Now + "==============================>");
                            Console.WriteLine("\n" + ex.ToString());
                        }
                    }
                    
                }

                if (input == "home")
                {

                    Console.Clear();
                    printLightning();
                    Console.WriteLine(" Version " + versionString);
                    Console.WriteLine("Welecome!\n");

                }

                if (input == "sysinf")
                {

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    print("RAM: " + Cosmos.Core.CPU.GetAmountOfRAM().ToString() + " bytes");
                    print("CPU Vendor: " + Cosmos.Core.CPU.GetCPUVendorName().ToString());
                    print("CPU Info: " + Cosmos.Core.CPU.GetCPUBrandString().ToString());
                    print("CPU Uptime: " + Cosmos.Core.CPU.GetCPUUptime().ToString() + " ms");
                    print("OS: " + "LightningOS Version " + versionString);
                    print("System Footprint: " + System.PlatformID.Other.ToString());

                }

                if(input == "drive")
                {
                    string[] filePaths = Directory.GetFiles(@"0:\");
                    var drive = new DriveInfo("0");
                    Console.WriteLine("Volume in drive 0 is " + $"{drive.VolumeLabel}");
                    Console.WriteLine("Directory of " + @"0:\");
                    Console.WriteLine("\n");
                    for(int i = 0; i < filePaths.Length; i++)
                    {
                        string path = filePaths[i];
                        Console.WriteLine(System.IO.Path.GetFileName(path));
                    }
                    foreach(var d in System.IO.Directory.GetDirectories(@"0:\"))
                    {
                        var dir = new DirectoryInfo(d);
                        var dirName = dir.Name;

                        Console.WriteLine(dirName + " <DIR>");
                    }

                    Console.WriteLine("\n");
                    Console.WriteLine("        " + $"{drive.TotalSize}" + " bytes");
                    Console.WriteLine("        " + $"{drive.AvailableFreeSpace}" + " bytes free");
                }

                if (input == "clock")
                {

                    //Clockapp.Run();

                }

                if (input == "sysconfig")
                {

                    handleSysPrefs();

                }


                if(input == "reboot")
                {
                    Sys.Power.Reboot();
                }
                ///
                ///End System Commands
                ///

                ///
                ///Start BYTE Commands
                ///

                if (input == "codeTest")
                {

                }

                if (input == "mcroblt -compile -run")
                {
                    Console.Write("Run file at path>");
                    string fileName = Console.ReadLine();
                    try
                    {
                       
                    }
                    catch
                    {
                        Console.WriteLine("Error: Unkown.");
                    }
                }

                if (input == "mcroblt")
                {
                    
                }

                if (input == "btaftrs")
                {
                    
                }

                //Games

                if(input == "snake")
                {
                    Snake.Program.snakeMain();
                   
                    
                    
                }

                if(input == "pong")
                {
                    PingPong.Program.runPong();
                }

                if(input == "breakout")
                {
                    
                }

                //End Games

               
                ///
                ///MicroBolt Commands
                ///

                if (input == "mcrblt")
                {
                    string pathToRun;
                    Console.Write("Enter path of file to run> ");
                    pathToRun = Console.ReadLine();
                    MicroBolt.Runtime.Run(pathToRun);
                }

                if (input == "mcrblt -cmd")
                {
                    MicroBolt.Runtime.CMDInterpreter();
                }
                ///
                ///End MicroBoltCommands
                ///

                if (input == "pwd")
                {
                    print(current_directory);
                }

                if(input == "prnt")
                {
                    Console.Write("PLEASE NOTE: This command is GLITCHY!!!");
                    Thread.Sleep(1000);
                    Console.Write("Enter path of file to print>");
                    string filepath = Console.ReadLine();
                    try
                    {
                        
                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            } else
            {

            }
            
        }







        ///
        ///End BYTE Commands
        ///


        public void StartGUI()
        {
            VGADriver driver;
       
            driver = new VGADriver();


            Screen s = new Screen();
            TextBox tbox = new TextBox(15, 100, 100);
            s.Controls.Add(tbox);

            driver.RenderScreen(s);
        

        

    }

        
    }




}



