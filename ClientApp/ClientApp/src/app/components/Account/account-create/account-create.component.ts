import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { UserCreateDto } from '../Models/accountDto';
import { NgForm } from '@angular/forms';
import { AccountService } from '../account.service';
import { HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
    selector: 'app-account-create',
    templateUrl: './account-create.component.html',
    styleUrls: ['./account-create.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class AccountCreateComponent implements OnInit {
  constructor(private serviceAccount: AccountService, private router:Router) { }


  userCreateDto: UserCreateDto = { email: '', password: '', name: '', codeArea: '', phoneNumber: '', identityNumber: '', lastName: '' };

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
