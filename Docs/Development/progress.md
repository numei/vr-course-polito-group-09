# A1 implementation progress

Plan: A1-plan.md. Initial repository: 75b9238. Existing clean branch: assignment-1.

Decision: work in the user's existing assignment-1 checkout; it has no Unity files or user modifications to isolate. No extra worktree is needed for this repository bootstrap.
Decision: use installed Unity 6.4 and custom controller, which satisfy A1's Unity 6.3-or-newer and new Input System requirements.
Decision: scale is an authored blockout plan derived from the PPT's three-zone layout, not a measured real station or a claim that the PPT contained dimensions.
Pre-flight: Task 1 scene path and Task 2 loader both use Assets/Scenes/Blockout.unity. Task 3 tests consume the saved scene rather than invoking a runtime environment generator.
Status: project settings scaffold copied from installed official URP template; new Input System only. References and tests next.

Scene tests: 4/4 passed. Initial PlayMode run: 3/8 passed because batch mode routed keyboard/mouse to editor. Diagnostic proved focus=False, controller captured=True, virtual W=0. Unity InputSettings source documents this default. Test fixture now routes virtual devices to the game, restores settings at teardown. No product workaround. Full PlayMode run: 8/8 passed.

Visual review: first rendered draft exposed oversized world-space TextMesh labels drawing across geometry. Removed world labels and used the existing screen legend for zone identity; geometry remains simple. This is a visual correction, not an art pass.
Full verification after correction: 4/4 EditMode and 10/10 PlayMode passed, including focus loss and 30/120 FPS stick speed. A0 link points to a clearly labelled local working draft seeded from supplied PPT because no shared document URL has been provided.

Task 1: complete — editable scene, reference images, saved prefab, editor acceptance.
Task 2: complete — Input System actions and controller, 10 passing runtime behaviors.
Task 3: fresh clone 08acb39 verified 14/14 with no Library, native Editor opened Blockout and entered Play. Native keyboard walk could not be asserted because automation loses its window connection on pointer capture; real device-event movement tests remain passing.
Final review: independent, no Critical/Important issues. Deferred minors: capture helper has no save prompt (README warns to save); copied URP default volume profile has unused missing Outline/Oasis subassets that URP prunes.
Builder judgment: provide repository-local A0 draft rather than invent a shared URL; group may replace link later. External teacher collaborator remains unverified due 403/unauthenticated browser.

Native follow-up: reacquiring the window after Play entry fixed keyboard automation access. Real W for 4.5 s reached AIRLOCK, W for 2 s reached EXTERIOR, S for 4.5 s returned to MISSION AREA. Native exterior screenshot saved. Escape / R / exit Play restored edit mode; Console shows 0 errors and 0 warnings, screenshot saved. Native mouse drag still unsupported by automation, so mouse look evidence remains the passing device-event tests.
Remote: d398376 pushed and matched ls-remote at 2026-11-09 17:17 Europe/Rome. Task 3: complete for implementation/test/push; teacher collaborator remains unverified external account state.
