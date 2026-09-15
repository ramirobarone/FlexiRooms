import { Component, OnInit } from '@angular/core';
import { UserCreateDto } from '../Models/accountDto';
import { NgForm } from '@angular/forms';
import { AccountService } from '../account.service';
import { HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-account-create',
  templateUrl: './account-create.component.html',
  styleUrls: ['./account-create.component.css']
})
export class AccountCreateComponent implements OnInit {
  constructor(private serviceAccount: AccountService, private router:Router) { }


  userCreateDto: UserCreateDto = { email: 'ramiro_barone@hotmail.com', password: 'ramiro0908', name: 'ramiro', codeArea: '351', phoneNumber: '7572518', identityNumber: '30331219', lastName: 'barone' };

  ngOnInit(): void {
  }

  createAccount(form: NgForm): void {
    if (form.invalid) {
      return;
    }

    console.log(this.userCreateDto);
    this.serviceAccount.createAccount(this.userCreateDto).subscribe({
      next: (res: boolean) => {
        if (res === true)
          this.router.navigate(['/login']);

          // Handle successful account creation
          console.log('Account created successfully', res);
      },
      error: (err) => {
        // Handle error case
        console.error('Error creating account', err);
      }
    });

  }
}
