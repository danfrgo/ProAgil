import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  titulo = 'Login';
  model: any = {};

  // inserir o router do html -> registar
  constructor(
    private authService: AuthService,
    public router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    // verificar se existem algo chamado toke e se é diferente de nulo -> caso afirmativo redireciona para a aba dashboard
    if (localStorage.getItem('token') != null) {
      this.router.navigate(['/dashboard']);
    }
  }

  login() {
    this.authService.login(this.model).subscribe(
      () => {
        // se tiver sucesso
        this.router.navigate(['/dashboard']);
        this.toastr.success('Login efetuado com sucesso');
      }, // se tiver erro
      (error) => {
        this.toastr.error('Falha ao tentar entrar na plataforma');
      }
    );
  }
}
