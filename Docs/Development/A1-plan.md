# Orbital Rescue A1 Implementation Plan

> **For agentic workers:** Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox syntax for tracking.

**Goal:** Deliver an editable reference-based space-station blockout and a working desktop first-person controller on the new Input System.

**Architecture:** A saved Unity scene contains primitive environment geometry and an instance of a player prefab. A separate editor command creates the scene and preview evidence. Input comes from an InputActionAsset, cloned per controller.

**Tech Stack:** Unity 6000.4.4f1, URP 17.4, Input System 1.17, Unity Test Framework.

**Spec:** [A1 design](A1-design.md) and the course A1 page linked in the README.

## Global Constraints
- Unity 6.3 or newer; Unity project at repository root.
- Scene path Assets/Scenes/Blockout.unity.
- Reference images in root Docs folder.
- New Input System only, desktop controller for A1.
- Delivery branch assignment-1 before 2026-11-15 23:59 Europe/Rome.
- Keep simple blockout volumes; VR locomotion and final assets belong to later assignments.

## Review Focus
- Fresh checkout resolves packages and opens without missing assets or scripts.
- Both doorways and all three zones are navigable with the full player capsule.
- Cursor release and focus loss stop movement; resume does not reuse stale mouse movement.
- Diagonal movement and gamepad look remain independent of frame rate.
- All required reference/document links and remote delivery claims have evidence.

### Task 1: Reference and saved environment
**Files:** Docs/References/*, ProjectSettings/*, Packages/*, Assets/Editor/BlockoutBuilder.cs, Assets/Scenes/Blockout.unity, Assets/Tests/Editor/BlockoutSceneTests.cs.
**Interfaces:** BlockoutBuilder.Build() saves a complete scene. Required hierarchy roots: Environment, Player, Lighting, Context.
- [ ] Save original concept images and authored dimensioned floor plan.
- [ ] Write scene acceptance tests, run and observe missing scene/reference failures.
- [ ] Build primitive rooms, open hatches and exterior equipment with colliders; persist scene and build settings.
- [ ] Run editor acceptance tests: no missing scripts/materials, one main camera, unobstructed complete route and safe spawn.

### Task 2: First-person input and navigation
**Files:** Assets/Input/OrbitalRescue.inputactions, Assets/Scripts/FirstPersonController.cs, Assets/Scripts/BlockoutGuide.cs, Assets/Prefabs/Player.prefab, Assets/Tests/PlayMode/FirstPersonTests.cs.
**Interfaces:** Controller consumes serialized InputActionAsset and child Camera; scene is loaded by name Blockout. Tests drive virtual Keyboard/Mouse/Gamepad devices through Input System events.
- [ ] Write PlayMode tests for movement, diagonal speed, wall collision, look/pitch, release/resume, reset and exterior traversal before controller implementation.
- [ ] Implement controller and input actions, generate player prefab into the scene.
- [ ] Run Unity PlayMode tests until all cases pass and visually inspect actual Play mode.

### Task 3: Submission and clean-checkout verification
**Files:** README.md, Docs/A1-checklist.md, Docs/Validation/*, Tools/*.
**Interfaces:** tools accept Unity editor path and project root; test XML and preview PNGs are written to ignored local artifacts or Docs/Validation as appropriate.
- [ ] Document launch instructions, controls, reference mapping and A0 link/status.
- [ ] Run import, full EditMode/PlayMode suites and visual checks, inspect outputs.
- [ ] Review source and requirements, resolve material issues.
- [ ] Commit on assignment-1; clone into a fresh empty directory and run import/tests without the existing Library.
- [ ] Push assignment-1 and verify GitHub SHA if available. Record collaborator/A0 status and all remaining external requirements in final checklist.
