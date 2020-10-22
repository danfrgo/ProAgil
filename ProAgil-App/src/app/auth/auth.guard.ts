import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

// redireciona para o login e assim os users sem o login efetuado nao podem entrar nas outras rotas/endereços/paginas HTML
export class AuthGuard implements CanActivate {

  constructor(private router: Router) {}

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {

    if (localStorage.getItem('token') !== null) { // se o token for diferente de null entao pode acessar
      return true;
    } else { // se o token estiver vazio -> redireciona para o login
      this.router.navigate(['/user/login']);
      return false;
    }
  }
}