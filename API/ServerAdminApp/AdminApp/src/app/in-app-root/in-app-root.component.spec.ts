import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InAppRootComponent } from './in-app-root.component';

describe('InAppRootComponent', () => {
  let component: InAppRootComponent;
  let fixture: ComponentFixture<InAppRootComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InAppRootComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InAppRootComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
