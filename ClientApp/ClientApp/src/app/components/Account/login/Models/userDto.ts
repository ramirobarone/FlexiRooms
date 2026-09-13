export interface UserDto {
  email: string;
  password: string;
}
export interface UserLoginDto {
  token: string;
  fullName: string;
  rolId: number;
  role: string;
  userGuid: string;
}

