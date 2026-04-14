import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SessionStorageService {

  set<T>(key: string, value: T) {
    sessionStorage.setItem(key, JSON.stringify(value));
  }

  get<T>(key: string): T | null {
    const data = sessionStorage.getItem(key);
    return data ? JSON.parse(data) : null;
  }

  remove(key: string) {
    sessionStorage.removeItem(key);
  }
}