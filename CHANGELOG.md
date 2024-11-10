# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
