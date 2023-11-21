using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DB {
	public int UserID;

	public UserClass.UserPermissions userPermissions => users[UserID].Permissions;

	public List<UserClass> users = new List<UserClass>();

	public List<StudentClass> students = new List<StudentClass>();
	public List<LecturerClass> lecturers = new List<LecturerClass>();

	public List<GroupClass> groups = new List<GroupClass>();

	public List<LectureClass> lectures = new List<LectureClass>();
	public List<GradeClass> grades = new List<GradeClass>();

	public int FindStudentID(string studentCode) {
		foreach (var student in students) { 
			if(student.StudentCode == studentCode)
				return students.IndexOf(student);
		}

		return -1;
	}
}

[Serializable]
public class UserClass {
	[SerializeField] private string Username;
	[SerializeField] private string Password;
	[SerializeField] public UserPermissions Permissions;

	public UserClass(string _u, string _p, UserPermissions access) { 
		Username = _u;
		Password = _p;
		Permissions = access;
	}

	public bool Login(string _u, string _p) {
		return Username == _u && Password == _p;
	}

	public enum UserPermissions { Admin, Lecturer, Student };
}

[Serializable]
public class GroupClass {
	public string GroupName;
}

[Serializable]
public class LectureClass {
	public string LectureName;
	public int GroupID;
	public int LecturerID;

	public LectureClass(string lectureName, int groupID, int lecturerID) {
		LectureName = lectureName;
		GroupID = groupID;
		LecturerID = lecturerID;
	}
}

[Serializable]
public class StudentClass {
	public string FirstName;
	public string LastName;

	public string StudentCode;

	public int GroupID;

	public StudentClass(string name, string surname, string code, int groupID = -1) {
		FirstName = name;
		LastName = surname;
		StudentCode = code;
		GroupID = groupID;
	}
}

[Serializable]
public class LecturerClass {
	public string FirstName;
	public string LastName;

	public LecturerClass (string name, string surname) {
		FirstName = name;
		LastName = surname;
	}
}

[Serializable]
public class GradeClass {
	public int LectureID;
	public int StudentID;

	public int Grade;

	public GradeClass(int lectureID, int studentID, int grade) {
		LectureID = lectureID;
		StudentID = studentID;
		Grade = grade;
	}
}
