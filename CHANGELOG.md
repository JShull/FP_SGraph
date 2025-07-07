# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.5.0] - 2025-07-07

FP_Installer support added and fixed small obsolete code requirement in Samples.

### 0.5.0 Changed

- [@JShull](https://github.com/jshull).
  - NodeMBExEditor.cs
    - 227: Object.FindObjectsByType<NodeMBEx>(FindObjectsSortMode.InstanceID);

## [0.4.0] - 2025-04-24

### 0.4.0 Changed

All sorts of major updates to the StateMachineSB.cs = this will have breaking changes on previous versions

- [@JShull](https://github.com/jshull).
  - StateMachineSB.cs
    - Reworked entire requirements tied to no longer just unlock but all transitions
  - FPEVManager.cs
    - Added in RequirementData list for all FPTransitionMapper structs
  - FPMonoEvent.cs
    - Gutted this to align to major changes at the StateMachineSB level and the FPEVmanager level

## [0.3.0] - 2025-03-11

### 0.3.0 Added

- [@JShull](https://github.com/jshull).
  - FPMonoEventTester.cs
    - This separates the testing functions and parameters from the FPMonoEvent class

### 0.3.0 Changed

- [@JShull](https://github.com/jshull).
  - StateMachineSB.cs
    - Mechanism to Invoke on Event Init via 'TryInvokeEventInitialization()
  - FPEVManager.cs
    - Wrapper function and logic to allow for the Invoked Event based on starting state
  - FPMonoEvent.cs
    - Removed the logic, parameters, and functions tied to testing related needs into a new mono class FPMonoEventTester.cs

## [0.2.0] - 2024-11-10

- [@JShull](https://github.com/jshull).
  - Updated Simple Setup
  - New Helper Configuration Added in
  - Extended FP_Data for a Threshold Configuration
  - Extended FP_Timer to have a 'Helper Timer'
    - This uses passed information by category and sequence state outcome to build a key
    - Threshold time and a max amount since last event is now managed
    - Still on the user in the configuration to add these in by the FPEventMono.cs in the editor

### 0.2.0 Added

- [@JShull](https://github.com/jshull).
  - FPEventMono.cs
  - FPEVManager.cs
  - FPHelperTimer.cs
  - FPMonoEvent.cs
  - HelperThresholdConfig.cs

## [0.1.0] - 2024-02-17

### 0.1.0 Added

- [@JShull](https://github.com/jshull).
  - Core SGraph Setup
  - Initial Setup

### 0.1.0 Fixed

- None...

### 0.1.0 Removed

- SGraph is it's own package - just not hosted yet

### 0.1.0 Changed

- None... yet
