export interface UserInformationDto {
  id: string;
  email: string | null;
  name: string | null;
  secondName: string | null;
  lastName: string | null;
  identityNumber: string | null;
  codeArea: string | null;
  phoneNumber: string | null;
  profileImageUrl: string | null;
}

export interface UpdateUserInformationDto {
  name: string | null;
  secondName: string | null;
  lastName: string | null;
  identityNumber: string | null;
  codeArea: string | null;
  phoneNumber: string | null;
}
