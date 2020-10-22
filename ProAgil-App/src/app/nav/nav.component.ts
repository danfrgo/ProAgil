import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {

  
  constructor(private authService: AuthService
    , public router: Router
    , private toastr: ToastrService) { }

  ngOnInit() {
  }

  loggedIn(){
    return this.authService.loggedIn();
  }

  entrar() {
    this.router.navigate(['/user/login']);
  }

  logout(){
    localStorage.removeItem('token'); // remover token
    this.toastr.show('Sessão encerrada');
    this.router.navigate(['/user/login']); // depois de finalizada a sessao, redireciono para o login
  }

}
