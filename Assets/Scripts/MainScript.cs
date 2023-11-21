using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class MainScript : MonoBehaviour {
	public List<GameObject> Menus = new List<GameObject>();

	public DB Database = new DB();

	[Header("References")]
	public LoginMenu loginMenu;
	public MainOptionsMenu mainOptionsMenu;

	[Header("Prefabs")]
	public GameObject PanelPrefab;
	public GameObject PopupPrefab;
	public GameObject RowPrefab;

	public static MainScript instance;

	private void Awake () {
		if (instance != null)
			Destroy(instance.gameObject);

		instance = this;
	}

	private void Start () {
		LoadMenu(0);

		Load();
	}

	public void Login () {
		foreach (var user in Database.users) {
			if (user.Login(loginMenu.UsernameInputField.text, loginMenu.PasswordInputField.text)) {
				Database.UserID = Database.users.IndexOf(user);

				LoadMenu(1);

				var buttonsBuilder = PanelBuilder.BuildPanel();

				switch (Database.userPermissions) {
					case UserClass.UserPermissions.Admin:
						buttonsBuilder.AddButton("Manage users", delegate {
							if (mainOptionsMenu.secondaryPanel != null)
								Destroy(mainOptionsMenu.secondaryPanel.gameObject);

							var userButtons = PanelBuilder.BuildPanel()
							.AddButton("Add student",       delegate { LoadMenu(2); })
							.AddButton("Remove student",    delegate { LoadMenu(3); })
							.AddButton("Add Lecturer",      delegate { LoadMenu(4); })
							.AddButton("Remove Lecturer",   delegate { LoadMenu(5); })
							.GetPanelButtons();

							mainOptionsMenu.secondaryPanel = CreatePanel("Manage users", userButtons);
						})
						.AddButton("Manage groups", delegate {
							if (mainOptionsMenu.secondaryPanel != null)
								Destroy(mainOptionsMenu.secondaryPanel.gameObject);

							var groupButtons = PanelBuilder.BuildPanel()
								.AddButton("Add group",       delegate { LoadMenu(6); })
								.AddButton("Remove group",    delegate { LoadMenu(7); })
								.AddButton("Assign student",  delegate { LoadMenu(8); })
								.GetPanelButtons();

							mainOptionsMenu.secondaryPanel = CreatePanel("Manage groups", groupButtons);
						})
						.AddButton("Manage lectures", delegate {
							if (mainOptionsMenu.secondaryPanel != null)
								Destroy(mainOptionsMenu.secondaryPanel.gameObject);

							var lectureButtons = PanelBuilder.BuildPanel()
								.AddButton("Add lecture",       delegate { LoadMenu(9); })
								.AddButton("Remove lecture",    delegate { LoadMenu(10); })
								.AddButton("Assign group",      delegate { LoadMenu(11); })
								.AddButton("Assign lecturer",   delegate { LoadMenu(12); })
								.GetPanelButtons();

							mainOptionsMenu.secondaryPanel = CreatePanel("Manage lectures", lectureButtons);
						});
						break;

					case UserClass.UserPermissions.Student:
						buttonsBuilder.AddButton("View grades", delegate { LoadMenu(13); });
						break;

					case UserClass.UserPermissions.Lecturer:
						buttonsBuilder.AddButton("Add grade", delegate { LoadMenu(14); })
									  .AddButton("Remove grade", delegate { LoadMenu(15); })
									  .AddButton("Modify grade", delegate { LoadMenu(16); });
						break;
				}

				CreatePanel("Select operation", buttonsBuilder.GetPanelButtons());

				return;
			}
		}

		CreatePopup("Error", "Invalid login details, please try again.", PopupScript.PopupType.Error);
	}

	public void LoadMenu (int menuID) {
		foreach (var menu in Menus) {
			menu.SetActive(false);
		}

		Menus[menuID].SetActive(true);
	}

	public PanelScript CreatePanel (string label, List<PanelButton> buttons) {
		var Panel = Instantiate(PanelPrefab, mainOptionsMenu.ItemContainer);

		Panel.name = label;

		var script = Panel.GetComponent<PanelScript>();

		script.Setup(label, buttons);

		return script;
	}

	public void CreatePopup (string title, string text, PopupScript.PopupType type = PopupScript.PopupType.Info) {
		var popup = Instantiate(PopupPrefab);

		popup.name = "Popup";

		var script = popup.GetComponent<PopupScript>();

		script.Setup(title, text, type);
	}

	public void Save () {
		string path = Application.dataPath + "/DB/Database.json";

		if (!Directory.Exists(path.Replace("/Database.json", "")))
			Directory.CreateDirectory(path.Replace("/Database.json", ""));

		string data = JsonUtility.ToJson(Database, true);

		File.WriteAllText(path, data);
	}

	public void Load () {
		string path = Application.dataPath + "/DB/Database.json";

		if (!File.Exists(path)) {
			Database = new DB();
			Database.users.Add(new UserClass("Admin", "Admin", UserClass.UserPermissions.Admin));
			Save();
		}

		string data = File.ReadAllText(path);

		Database = JsonUtility.FromJson<DB>(data);
	}


	#region Manage Users

	public void AddStudent (string FirstName, string LastName, int GroupID) {
		Database.users.Add(new UserClass(FirstName, LastName, UserClass.UserPermissions.Student));
		Database.students.Add(new StudentClass(FirstName, LastName, CreateStudentCode(), GroupID));

		CreatePopup("Student created!", "Student has been successfully created. Their logins are\nUsername: " + FirstName + "\nPassword: " + LastName, PopupScript.PopupType.Info);
	}

	public void RemoveStudent (int studentID) {
		var student = Database.students[studentID];

		foreach (var user in Database.users) {
			if (user.Login(student.FirstName, student.LastName))
				Database.users.Remove(user);
		}
		Database.students.RemoveAt(studentID);
	}

	public void AddLecturer (string FirstName, string LastName) {
		Database.users.Add(new UserClass(FirstName, LastName, UserClass.UserPermissions.Lecturer));
		Database.lecturers.Add(new LecturerClass(FirstName, LastName));

		CreatePopup("Lecturer created!", "Lecturer has been successfully created. Their logins are\nUsername: " + FirstName + "\nPassword: " + LastName, PopupScript.PopupType.Info);
	}

	public void RemoveLecturer (int LecturerID) {
		var lecturer = Database.lecturers[LecturerID];

		foreach (var user in Database.users) {
			if (user.Login(lecturer.FirstName, lecturer.LastName))
				Database.users.Remove(user);
		}
		Database.students.RemoveAt(LecturerID);
	}

	public string CreateStudentCode () {
		var code = "s" + Random.Range(0, 1000000).ToString("######");

		foreach (var student in Database.students) {
			if (student.StudentCode == code)
				return CreateStudentCode();
		}

		return code;
	}

	#endregion

	#region Manage Groups

	public void AddGroup (string groupName) {
		Database.groups.Add(new GroupClass { GroupName = groupName });
	}

	public void RemoveGroup (int groupID) {
		Database.groups.RemoveAt(groupID);
	}

	public void AssignStudent (int groupID, int studentID) {
		Database.students[studentID].GroupID = groupID;
	}

	#endregion

	#region Manage Lectures

	public void AddLecture (string lectureName, int lecturerID, int groupID) {
		Database.lectures.Add(new LectureClass(lectureName, lecturerID, groupID));
	}

	public void RemoveLecture (int lectureID) {
		Database.lectures.RemoveAt(lectureID);
	}

	public void AssignGroup (int lectureID, int groupID) {
		Database.lectures[lectureID].GroupID = groupID;
	}

	public void AssignLecturer (int lectureID, int lecturerID) {
		Database.lectures[lectureID].LecturerID = lecturerID;
	}

	#endregion

	#region Manage Grades

	public void AddGrade(int LectureID, int StudentID, int grade) {
		Database.grades.Add(new GradeClass(LectureID, StudentID, grade));
	}

	public void RemoveGrade(int GradeID) {
		Database.grades.RemoveAt(GradeID);
	}

	public void ModifyGrade(int GradeID, int grade) {
		Database.grades[GradeID].Grade = grade;
	}

	#endregion
}

[System.Serializable]
public class LoginMenu {
	public TMP_InputField UsernameInputField;
	public TMP_InputField PasswordInputField;
}

[System.Serializable]
public class MainOptionsMenu {
	public Transform ItemContainer;
	public PanelScript secondaryPanel;
}