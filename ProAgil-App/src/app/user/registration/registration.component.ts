import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/_models/User';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss'],
})
export class RegistrationComponent implements OnInit {
  registerForm: FormGroup;
  user: User;

  // para injetar
  constructor(
    private authService: AuthService,
    public router: Router,
    public fb: FormBuilder,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.validation();
  }

  validation() {
    this.registerForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]], // 2 validaçoes - 2 colchetes
      userName: ['', Validators.required],
      passwords: this.fb.group(
        {
          password: ['', [Validators.required, Validators.minLength(4)]],
          confirmPassword: ['', Validators.required],
        },
        { validator: this.compararSenhas }
      ),
    });
  }

  compararSenhas(fb: FormGroup) {
    const confirmSenhaCtrl = fb.get('confirmPassword');
    if (
      confirmSenhaCtrl.errors == null ||
      'mismatch' in confirmSenhaCtrl.errors
    ) {
      if (fb.get('password').value !== confirmSenhaCtrl.value) {
        confirmSenhaCtrl.setErrors({ mismatch: true });
      } else {
        confirmSenhaCtrl.setErrors(null);
      }
    }
  }

  cadastrarUsuario() {
    if (this.registerForm.valid) {
      // analisar se o formulario esta validado
      this.user = Object.assign(
        // se tiver validado pego o User do form
        { password: this.registerForm.get('passwords.password').value }, // tem que fazer match com os campos do metodo validation()
        this.registerForm.value
      );
      // console.log(this.user);
      this.authService.register(this.user).subscribe(
        () => {
          // se tiver sucesso
          this.router.navigate(['/user/login']);
          this.toastr.success('Registo Realizado');
        },
        (error) => {
          // caso tenha erro
          const erro = error.error;
          erro.forEach((element) => {
            switch (element.code) {
              case 'DuplicateUserName':
                this.toastr.error('Registo Duplicado!');
                break;
              default:
                this.toastr.error(`Erro no Registo! CODE: ${element.code}`); // aspas ´´ para template string
                break;
            }
          });
        }
      );
    }
  }
}
