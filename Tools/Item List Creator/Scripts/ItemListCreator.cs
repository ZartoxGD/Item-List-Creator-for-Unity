using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;

namespace ItemListCreator
{
    public class ItemListCreator : EditorWindow
    {

        //RAJOUTER UNE OU DEUX VRAIS TEXTURES

        #region Vars

        //The opened file
        private Object actualFile;
        //The path of the opened File
        private string actualFilePath;
        //The item selected in the list view
        private Item selectedItem;
        //The item list retrieved and converted from 'actualFile'
        private List<Item> items = new List<Item>();
        private bool hasFileOpen;

        //File Selection
        private Button createItemListButton;
        private ObjectField itemListFileSelector;

        //Show Item List
        private ListView itemListContainer;

        //Item Properties
        private IntegerField idField;
        private TextField nameField;
        private IntegerField maxStackField;
        private Toggle craftToggleField;
        private DropdownField itemCategoryField;
        private Button modifyItemButton;
        private Button createItemButton;
        private TextField descriptionField;

        //Actions zone
        private Button deselectItemButton;
        private Button deleteItemButton;
        private Button checkIdsButton;
        private Button autoIdsButton;
        private Button saveFileButton;
        private Button sortByIdButton;
        private Button sortByNameButton;
        private Button exportCsvButton;

        //Progress Bar
        private ProgressBar progressBar;

        //Item Icon
        private VisualElement iconHolder;
        private VisualElement iconBg;
        private ObjectField iconSelector;

        #endregion

        #region Initialisation

        [MenuItem("Tools/Item List Creator")]
        public static void OpenEditorWindow()
        {
            //Instantiate the window and set the information about it
            ItemListCreator window = GetWindow<ItemListCreator>();
            window.titleContent = new GUIContent("Item List Creator");
            window.maxSize = new Vector2(1000, 800);
            window.minSize = window.maxSize;
        }

        private void CreateGUI()
        {
            //Set the visual and assign the UXML file
            VisualElement root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tools/Item List Creator/Resources/ItemListCreator.uxml");
            VisualElement tree = visualTree.Instantiate();
            root.Add(tree);

            //The next section is making queries to assign variables
            //Initialise variables:

            //File Selection
            createItemListButton = root.Q<Button>("create-new-item-list-button");
            itemListFileSelector = root.Q<ObjectField>("json-load-field");

            // Show List
            itemListContainer = root.Q<ListView>("item-list-container");

            //Item Properties 
            idField = root.Q<IntegerField>("id-field");
            nameField = root.Q<TextField>("name-field");
            maxStackField = root.Q<IntegerField>("max-stackable-field");
            craftToggleField = root.Q<Toggle>("can-craft-field");
            itemCategoryField = root.Q<DropdownField>("category-field");
            modifyItemButton = root.Q<Button>("modify-item-button");
            createItemButton = root.Q<Button>("create-item-button");
            descriptionField = root.Q<TextField>("description-field");

            //Actions Zone
            deselectItemButton = root.Q<Button>("deselect-item-button");
            deleteItemButton = root.Q<Button>("delete-item-button");
            checkIdsButton = root.Q<Button>("check-ids-button");
            autoIdsButton = root.Q<Button>("auto-ids-button");
            saveFileButton = root.Q<Button>("save-file-button");
            sortByIdButton = root.Q<Button>("sort-by-id-button");
            sortByNameButton = root.Q<Button>("sort-by-name-button");
            exportCsvButton = root.Q<Button>("export-csv-button");

            //ProgressBar
            progressBar = root.Q<ProgressBar>("progress-bar");

            //Item Icon
            iconHolder = root.Q<VisualElement>("item-icon-holder");
            iconSelector = root.Q<ObjectField>("icon-file-selector");
            iconBg = root.Q<VisualElement>("item-icon-bg");

            //The next section sets the different callbacks and events
            //Callbacks

            //File Selection
            createItemListButton.clicked += () => CreateNewItemList();
            itemListFileSelector.RegisterValueChangedCallback<Object>(LoadFile);

            //Show List
            itemListContainer.onSelectionChange += ListViewSelectionChanged;

            //Item Properties
            modifyItemButton.clicked += () => ModifyItem();
            createItemButton.clicked += () => CreateItem();

            //Actions Zone
            deselectItemButton.clicked += () => DeselectItem();
            deleteItemButton.clicked += () => DeleteItem();
            checkIdsButton.clicked += () => CheckIds();
            autoIdsButton.clicked += () => AutoIds();
            saveFileButton.clicked += () => SaveFile();
            sortByIdButton.clicked += () => SortByID();
            sortByNameButton.clicked += () => SortByName();
            exportCsvButton.clicked += () => ExportToCSV();

            //Item Icon 
            iconSelector.RegisterValueChangedCallback<Object>(SetItemIcon);

            //Initialise hasOpenFile to false
            hasFileOpen = false;

            Texture2D bg = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Tools/Item List Creator/Resources/TransparentBG.png", typeof(Texture2D));
            iconBg.style.backgroundImage = bg;

            SetCategoryDropdownOptions();
            HideProgressBar();
        }

        #endregion

        #region Checks

        /// <summary>
        /// Check if the file located at 'path' Exist and if it is a .txt
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <returns>Return true if the file exists and is a txt / Return false if the file does not exist or if it's not a txt</returns>
        private bool CheckFileExistAndTxt(string path)
        {
            // Check if the file exists or if the path leads to nothing
            if (File.Exists(path))
            {
                // Get the file extension at the given path (e.g. for "text.txt", fileExtension = ".txt")
                string fileExtension = Path.GetExtension(path);

                // Check if the file is a text file
                if (fileExtension == ".txt")
                {
                    return true;
                }
                else
                {
                    // Log an error message if the file is not a text file
                    Debug.Log("The file is not a text file.");
                    return false;
                }
            }
            else
            {
                // Log an error message if the file does not exist
                Debug.Log("The file does not exist.");
                return false;
            }
        }

        /// <summary>
        /// Check if the ID passed in is equal to any itemID in the items List 
        /// </summary>
        /// <param name="id">The id to check.</param>
        /// <returns>Return true if ID is == to another itemID / Return false if ID != all itemID</returns>
        private bool CheckIdAlreadyExist(int id)
        {
            foreach (Item item in items)
            {
                if (item.Id == id)
                    return true;
            }
            return false;
        }

        #endregion

        #region Item Properties

        /// <summary>
        /// Change the fields in the item properties panel and set them to 'item' properties
        /// </summary>
        /// <param name="item">The value in this item will replace the field's values</param>
        private void ChangeItemProperties(Item item)
        {
            idField.SetValueWithoutNotify(item.Id);
            nameField.SetValueWithoutNotify(item.Name);
            maxStackField.SetValueWithoutNotify(item.MaxStackable);
            craftToggleField.SetValueWithoutNotify(item.CanCraft);
            descriptionField.SetValueWithoutNotify(item.Description);

            var optionValues = Enum.GetValues(typeof(ItemCategory));
            int optionIndex = Array.IndexOf(optionValues, item.Category);
            itemCategoryField.index = optionIndex;
        }

        /// <summary>
        /// Check if the selected item is null.
        /// </summary>
        /// <returns>return true if it's null / return false if it's not null</returns>
        private bool IsSelectedItemEmpty()
        {
            if (selectedItem == null)
            {
                Debug.Log("You need to select an item first.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set the category dropdown's (in Properties panel) options to the chosen enum
        /// </summary>
        private void SetCategoryDropdownOptions()
        {
            var options = new List<string>();

            //If you want to change or create new item categories: simply go to the 'enums' file and modify the enum, or create a new enum and change 
            //the typeof(X) with your enum
            foreach (var option in Enum.GetValues(typeof(ItemCategory)))
            {
                options.Add(option.ToString());
            }
            itemCategoryField.choices = options;
        }

        /// <summary>
        /// Modify the selected item with the values in Item Properties's fields
        /// </summary>
        private void ModifyItem()
        {
            if (!IsSelectedItemEmpty() && hasFileOpen)
            {
                selectedItem.Id = idField.value;
                selectedItem.Name = nameField.value;
                selectedItem.MaxStackable = maxStackField.value;
                selectedItem.CanCraft = craftToggleField.value;
                selectedItem.Category = (ItemCategory)itemCategoryField.index;
                selectedItem.Description = descriptionField.value;
                itemListContainer.Rebuild();
            }
        }

        /// <summary>
        /// Create a new item with the values in Item Properties fields 
        /// </summary>
        private void CreateItem()
        {
            if (hasFileOpen)
            {
                try
                {
                    //Get the enum
                    ItemCategory cat = (ItemCategory)Enum.Parse(typeof(ItemCategory), itemCategoryField.value);
                    //Create Item
                    Item newItem = new Item(idField.value, nameField.value, descriptionField.value, craftToggleField.value, cat, maxStackField.value);

                    //Check if the ID chosen is already taken
                    if (!CheckIdAlreadyExist(newItem.Id))
                    {
                        items.Add(newItem);
                        itemListContainer.Rebuild();
                    }
                    else
                    {
                        Debug.LogWarning($"ID:{newItem.Id} is already in Item List. The ID is unique to every item: please change the ID field.");
                        items.Add(newItem);
                        itemListContainer.Rebuild();
                    }
                }
                catch(ArgumentNullException)
                {
                    Debug.LogWarning("Item Creation failed! Please change the defaults values in 'Item Properties'");
                }
                catch (Exception ex)
                {
                    Debug.LogError("An error occurred: " + ex.Message);
                }

            }
            else
                Debug.Log("You have to open a file first.");
        }

        /// <summary>
        /// Set the fields in Item Properties to default values
        /// </summary>
        private void RefreshItemProperties()
        {
            idField.value = -1;
            nameField.value = "name";
            descriptionField.value = "description";
            craftToggleField.value = false;
            maxStackField.value = 0;
            itemCategoryField.index = -1;
        }

        /// <summary>
        /// Choose to show the create button or the modify button
        /// </summary>
        /// <param name="val">if true: show the create button / if false: show the modify button</param>
        private void AfficherCreateBtn(bool val)
        {
            if (val)
            {
                createItemButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                modifyItemButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
            else
            {
                createItemButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                modifyItemButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            }
        }

        #endregion

        #region ItemIcon

        //Event: used when the user changes the file in the iconSelector
        //Set the selectedItem Icon to the file referenced in iconSelector
        private void SetItemIcon(ChangeEvent<Object> evt)
        {
            // Check if a file is open and if an item is selected
            if (hasFileOpen && selectedItem != null)
            {
                // Set the selected item's icon to the chosen icon
                selectedItem.Icon = iconSelector.value as Texture2D;
                // Update the icon display to show the chosen icon
                iconHolder.style.backgroundImage = selectedItem.Icon;
            }
            else
                // Clear the icon selector if no file is open or no item is selected
                iconSelector.value = null;
        }

        /// <summary>
        /// Set icon Preview to the icon of the selected icon, if item.Icon == null: set a predefined texture
        /// </summary>
        private void LoadItemIcon()
        {
            // Check if the selected item has an icon set
            if (selectedItem.Icon != null)
                // Display the selected item's icon
                iconHolder.style.backgroundImage = selectedItem.Icon;
            else
            {
                // Load the predefined icon if the selected item does not have an icon set
                Texture2D noTexIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Tools/Item List Creator/Resources/NoTexture.png", typeof(Texture2D));
                // Display the "no texture" icon
                iconHolder.style.backgroundImage = noTexIcon;
            }
        }

        #endregion

        #region Actions Zone

        /// <summary>
        /// Delete the selected item from the list and rebuild the list/item properties panel.
        /// </summary>
        private void DeleteItem()
        {
            // Check if a file is open
            if (hasFileOpen)
            {
                // Remove the selected item from the list and clear the selected item and icon properties
                items.Remove(selectedItem);
                selectedItem = null;
                RefreshItemProperties();
                itemListContainer.Rebuild();
                iconHolder.style.backgroundImage = null;
                iconSelector.value = null;

                // Show the "Create" button and hide the "Modify" button
                AfficherCreateBtn(true);
            }
            else
                // Log an error message if no file is open
                Debug.Log("You need to open a file first.");
        }

        /// <summary>
        /// Deselect the actual item
        /// </summary>
        private void DeselectItem()
        {
            // Check if a file is open
            if (hasFileOpen)
            {
                // Clear the item properties, deselect the item in the list view, and clear the icon properties
                AfficherCreateBtn(true);
                RefreshItemProperties();
                List<int> indices = new List<int> { -1 };
                itemListContainer.SetSelectionWithoutNotify(indices);
                selectedItem = null;
                iconHolder.style.backgroundImage = null;
                iconSelector.value = null;
            }
            else
                // Log an error message if no file is open
                Debug.Log("You need to open a file first.");
        }

        /// <summary>
        /// Check if any ID of the list is unique. Send the result in the Console.
        /// </summary>
        private void CheckIds()
        {
            // Check if a file is open
            if (hasFileOpen)
            {
                // Loop through each item in the list and set the progress bar up
                int index = 0;
                ShowProgressBar(items.Count, "IDs Check");
                foreach (Item item in items)
                {
                    int id = item.Id;
                    index++;
                    ChangeProgressBarValue(index);
                    // Check if any other item in the list has the same id
                    foreach (Item it in items)
                    {
                        if (item != it)
                        {
                            if (it.Id == id)
                            {
                                // Log a warning if an id conflict is found
                                Debug.LogWarning($"ID conflict between: {item.Name}({id}) and {it.Name}({it.Id})");
                                HideProgressBar();
                                return;
                            }
                        }
                    }
                }
                Debug.Log("ID Check Completed: No errors");
                HideProgressBar();
            }
            else
            {
                // Log an error message if no file is open
                Debug.Log("You have to open a file first.");
            }
        }

        /// <summary>
        ///Assign automatically every item's ID. The first item will have 0, 2nd 1, 3rd 2, etc...
        /// </summary>
        private void AutoIds()
        {
            // Check if a file is open
            if (hasFileOpen)
            {
                // Setup the progress bar and set the id of each item to its index in the list
                ShowProgressBar(items.Count, "Auto IDs");
                for (int i = 0; i < items.Count; i++)
                {
                    ChangeProgressBarValue(i);
                    items[i].Id = i;
                }
                // Rebuild the list view to reflect the updated ids
                itemListContainer.Rebuild();
                Debug.Log("Auto IDs Completed!");
                HideProgressBar();
            }
            else
                // Log an error message if no file is open
                Debug.Log("You need to open a file first.");
        }

        /// <summary>
        /// Save the actual file.
        /// </summary>
        private void SaveFile()
        {
            // Check if a file is open
            if (hasFileOpen)
            {
                // Sort the items by id
                SortByID();
                // Convert the list of items to an array
                Item[] itemsToSave = items.ToArray();
                // Convert the item array to a JSON string
                string json = JsonHelper.ToJson<Item>(itemsToSave, true);
                // Write the JSON string to the open file
                File.WriteAllText(actualFilePath, json);
                // Log a message to indicate that the file has been saved
                Debug.Log("File Saved!");
            }
            else
                // Log an error message if no file is open
                Debug.Log("You need to open a file first.");
        }

        /// <summary>
        /// Sort the displayed list by ID (< to >)
        /// </summary>
        private void SortByID()
        {
            // Check if a file is open
            if (hasFileOpen)
            {
                // Sort the items by id
                items.Sort((i1, i2) => i1.Id.CompareTo(i2.Id));
                // Rebuild the list view to reflect the sorted order
                itemListContainer.Rebuild();
            }
            else
                // Log an error message if no file is open
                Debug.Log("You need to open a file first.");
        }

        /// <summary>
        /// Sort the displayed list alphabetically
        /// </summary>
        private void SortByName()
        {
            // Check if a file is open
            if (hasFileOpen)
            {
                // Sort the items by name
                items.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));
                // Rebuild the list view to reflect the sorted order
                itemListContainer.Rebuild();
            }
            else
                // Log an error message if no file is open
                Debug.Log("You need to open a file first.");
        }

        /// <summary>
        /// Export the current file to a csv and open a file dialog to get the save path
        /// </summary>
        public void ExportToCSV()
        {
            // Check if a file is open
            if (hasFileOpen)
            {
                // Open a save file panel to choose the destination for the CSV file
                var path = EditorUtility.SaveFilePanel("Export Item List To CSV", Application.dataPath, "newItemList.csv", "csv");
                // Return if the user cancels the save file panel
                if (string.IsNullOrEmpty(path))
                    return;
                // Write the items to the CSV file
                using (StreamWriter sw = File.CreateText(path))
                {
                    // Write the column headers to the file
                    sw.WriteLine("Id;Name;Description;Craft;Category;MaxStackable");
                    // Write each item to the file
                    foreach (Item item in items)
                    {
                        string line = string.Format("{0};{1};{2};{3};{4};{5}", item.Id, item.Name, item.Description, item.CanCraft, item.Category, item.MaxStackable);
                        sw.WriteLine(line);
                    }
                }
                // Refresh the asset database
                AssetDatabase.Refresh();
                // Log a message to indicate that the file has been exported
                Debug.Log("File Exported to CSV!");
            }
            else
                // Log an error message if no file is open
                Debug.Log("You need to open a file first.");
        }

        #endregion

        #region Items List

        /// <summary>
        /// Retrieve every Item in the selected file and set the itemsList up
        /// </summary>
        private void RetrieveAllItemsFromJson()
        {
            // Read the contents of the open file
            string json = File.ReadAllText(actualFilePath);
            // Check if the file is empty
            if (json.Length == 0 || json == "")
            {
                // Log a message if the file is empty
                Debug.Log("File is empty.");
            }
            else
            {
                try
                {
                    // Convert the JSON string to an array of items
                    Item[] itemsArray = JsonHelper.FromJson<Item>(json);

                    // Add each item to the items list
                    foreach (Item item in itemsArray)
                    {
                        items.Add(item);
                    }
                }
                catch(ArgumentException ex)
                {
                    Debug.LogWarning("Unable to open this file:");
                    Debug.LogWarning(ex.Message);
                    return;
                }
                catch (Exception ex)
                {
                    Debug.LogError("An error occurred: " + ex.Message);
                }
            }
            // Show the item list
            ShowItemList();
        }

        /// <summary>
        /// Bind the List View to the items list
        /// </summary>
        private void ShowItemList()
        {
            itemListContainer.makeItem = () => new Label();
            itemListContainer.bindItem = (e, i) => (e as Label).text = $"{items[i].Name} (ID:{items[i].Id}): {items[i].Category}";
            itemListContainer.itemsSource = items;
            itemListContainer.fixedItemHeight = 16;
        }

        // Event: called when user select a new element from the list view
        // Set: the modify button, and the fields in Item Properties 
        private void ListViewSelectionChanged(IEnumerable<object> objects)
        {
            AfficherCreateBtn(false);

            selectedItem = (Item)objects.ElementAt(0); ;
            ChangeItemProperties(selectedItem);
            LoadItemIcon();
            //iconSelector.value = null;
        }

        #endregion

        #region File Selection

        /// <summary>
        /// Create a new txt file at a path chosen by the user
        /// </summary>
        private void CreateNewItemList()
        {
            // Open a save file panel to choose the location for the new item list file
            var path = EditorUtility.SaveFilePanel("Create new Item List", Application.dataPath, "newItemList.txt", "txt");
            // Return if the user cancels the save file panel
            if (string.IsNullOrEmpty(path))
                return;
            // Create an empty file at the specified location
            File.WriteAllText(path, string.Empty);
            // Get the file path relative to the Assets folder
            string pathString = path;
            int assetIndex = pathString.IndexOf("Assets", StringComparison.Ordinal);
            string filePath = pathString.Substring(assetIndex, path.Length - assetIndex);
            // Import the file into the Asset Database
            AssetDatabase.ImportAsset(filePath);
            // Refresh the Asset Database
            AssetDatabase.Refresh();
            // Focus the Project Window
            EditorUtility.FocusProjectWindow();
            // Log a message to indicate that the new item list has been created
            Debug.Log("Succesful creation of a new Item List at: " + filePath);
            // Clear the items list
            items.Clear();
            // Set the actual file path and actual file variables
            actualFilePath = filePath;
            actualFile = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
            // Rebuild the list view
            itemListContainer.Rebuild();
            // Update the item list
            ChangeItemList(filePath);
        }

        /// <summary>
        /// Set the value of itemListFileSelector the file at the path sent
        /// </summary>
        /// <param name="filePath">the location of the path</param>
        private void ChangeItemList(string filePath)
        {
            if (CheckFileExistAndTxt(filePath))
            {
                TextAsset file = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
                itemListFileSelector.value = file;
            }
        }

        //Event: called when fileSelector's value is changed.
        //Set: the actual file used
        private void LoadFile(ChangeEvent<Object> evt)
        {
            // Return if no file has been selected
            if (evt.newValue == null)
                return;
            // Set the actual file and its file path
            actualFile = evt.newValue;
            actualFilePath = AssetDatabase.GetAssetPath(actualFile);
            // Check if the file exists and is a text file
            if (CheckFileExistAndTxt(actualFilePath))
            {
                // Set hasFileOpen to true
                hasFileOpen = true;
                // Log a message indicating the selected file and its path
                Debug.Log("Selected file is now: " + actualFile.name + " from: " + actualFilePath);
                // Clear the items list and retrieve all items from the file
                items.Clear();
                RetrieveAllItemsFromJson();
                // Rebuild the list view
                itemListContainer.Rebuild();
            }
            else
            {
                // Reset the object field, actual file, and actual file path if the file does not exist or is not a text file
                itemListFileSelector.value = null;
                actualFile = null;
                actualFilePath = null;
                // Set hasFileOpen to false
                hasFileOpen = false;
            }
        }

        #endregion

        #region Progress Bar

        //Will be shown only if the item list is really gigantic... Else no operation is slow enough to make it appear

        /// <summary>
        /// Show the progress bar and set is value.
        /// </summary>
        /// <param name="highValue">The highest value the bar can display (100%)</param>
        /// <param name="text">The text to be displayed in the bar while loading</param>
        private void ShowProgressBar(int highValue, string text)
        {
            progressBar.value = 0;
            progressBar.title = text;
            progressBar.highValue = highValue;
            progressBar.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Change the proggress bar's value.
        /// </summary>
        /// <param name="value">The value to set the progress bar to</param>
        private void ChangeProgressBarValue(int value)
        {
            progressBar.value = value;
        }

        /// <summary>
        /// Hide the progress bar
        /// </summary>
        private void HideProgressBar()
        {
            progressBar.style.display = DisplayStyle.None;
            progressBar.value = 0;
        }

        #endregion

    }
}


