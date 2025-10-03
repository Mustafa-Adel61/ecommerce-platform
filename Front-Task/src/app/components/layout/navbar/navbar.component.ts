import { Component, OnInit } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule,NgIf],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  isAuthenticated$: Observable<boolean>;
  isLoggingOut = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
  }

  ngOnInit(): void {}

  logout(): void {
    this.isLoggingOut = true;
    
    this.authService.logout().subscribe({
      next: () => {
        this.isLoggingOut = false;
        // Navigate to login page
        this.router.navigate(['/login']);
      },
      error: (error) => {
        this.isLoggingOut = false;
        console.error('Logout error:', error);
        // Even if there's an error, navigate to login since tokens are cleared locally
        this.router.navigate(['/login']);
      }
    });
  }
}
