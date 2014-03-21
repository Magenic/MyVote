
declare module MyVote.AppServer.Models {
interface PollSummary {
  Category: string;
  Id: number;
  ImageLink: string;
  Question: string;
  SubmissionCount: number;
}
interface Poll {
  PollID: number;
  UserID: number;
  PollCategoryID: number;
  PollQuestion: string;
  PollImageLink: string;
  PollMaxAnswers: number;
  PollMinAnswers: number;
  PollStartDate: Date;
  PollEndDate: Date;
  PollAdminRemovedFlag: boolean;
  PollDateRemoved: Date;
  PollDeletedFlag: boolean;
  PollDeletedDate: Date;
  PollDescription: string;
  PollOptions: MyVote.AppServer.Models.PollOption[];
}
interface PollOption {
  PollOptionID: number;
  PollID: number;
  OptionPosition: number;
  OptionText: string;
}
interface PollResponse {
  PollID: number;
  UserID: number;
  Comment: string;
  ResponseItems: MyVote.AppServer.Models.ResponseItem[];
}
interface ResponseItem {
  PollOptionID: number;
  IsOptionSelected: boolean;
}
interface PollResult {
  PollID: number;
  IsPollOwnedByUser: boolean;
  IsActive: boolean;
  PollImageLink: string;
  Question: string;
  Results: MyVote.AppServer.Models.PollResultItem[];
  Comments: MyVote.AppServer.Models.PollResultComment[];
}
interface PollResultItem {
  OptionText: string;
  PollOptionID: number;
  ResponseCount: number;
}
interface PollResultComment {
  PollID: number;
  PollCommentID: number;
  ParentCommentID: number;
  CommentDate: Date;
  CommentText: string;
  Comments: MyVote.AppServer.Models.PollResultComment[];
  UserName: string;
  UserID: number;
}
interface PollInfo {
  PollSubmissionID: number;
  PollID: number;
  PollDescription: string;
  PollQuestion: string;
  MaxAnswers: number;
  MinAnswers: number;
  UserID: number;
  SubmissionDate: Date;
  Comment: string;
  PollOptions: MyVote.AppServer.Models.PollResponseOption[];
}
interface PollResponseOption {
  PollResponseID: number;
  PollOptionID: number;
  IsOptionSelected: boolean;
  OptionPosition: number;
  OptionText: string;
}
interface User {
  ProfileID: string;
  UserID: number;
  UserName: string;
  FirstName: string;
  LastName: string;
  PostalCode: string;
  Gender: string;
  EmailAddress: string;
  BirthDate: Date;
}
}


declare module MyVote.AppServer.Models {
	interface PollOption {
		Selected: boolean;
	}
}
