import { TestBed } from '@angular/core/testing';

import { DeviceActivityService } from './device-activity.service';

describe('DeviceActivityService', () => {
  let service: DeviceActivityService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeviceActivityService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
