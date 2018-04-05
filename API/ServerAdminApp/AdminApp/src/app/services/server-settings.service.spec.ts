import { TestBed, inject } from '@angular/core/testing';

import { ServerSettingsService } from './server-settings.service';

describe('ServerSettingsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ServerSettingsService]
    });
  });

  it('should be created', inject([ServerSettingsService], (service: ServerSettingsService) => {
    expect(service).toBeTruthy();
  }));
});
