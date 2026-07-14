# Assignment – 1
## Git Assignment: Collaborate and Manage a Project

**Name:** Ansh Verma  

**Repository Link:**  
🔗 https://github.com/AnshVerma-ITT/SDET_Ansh_Verma

**Bootstrap Repository Link:**  
🔗 https://github.com/AnshVerma-ITT/bootstrap

---

# Objective

The objective of this assignment is to practice and demonstrate the use of essential Git and GitHub commands used in real-world software development projects.

## Commands Covered

- Fork
- Clone
- Branch Creation
- Checkout
- Add
- Commit
- Push
- Pull Request
- Stash
- Merge
- Cherry Pick
- Rebase
- Tag Creation

---

# 1. Installing Git and Configuring Username & Email

Configured Git username and email using:

```bash
git config --global user.name "Ansh Verma"
git config --global user.email "ansh.verma@intimetec.com"
git config --list
```

---

# 2. Original Bootstrap Repository

![Original Repository](screenshots/original_repository.png)

---

# 3. Forked Repository

![Forked Repository](screenshots/forked_repository.png)

---

# 4. Cloning the Repository

```bash
git clone https://github.com/AnshVerma-ITT/bootstrap.git
```

![Clone Repository](screenshots/clone.png)

---

# 5. Opening Bootstrap Project in VS Code

![VS Code](screenshots/vscode_bootstrap_proj.png)

---

# 6. Creation of Feature Branch

```bash
git checkout -b feature
```

![Feature Branch](screenshots/feature_branch.png)

---

# 7. Edited README.md

![Updated README](screenshots/changed_featurereadme.png)

---

# 8. Checking Git Status

```bash
git status
```

![Git Status](screenshots/git_status.png)

---

# 9. Adding and Committing Changes

```bash
git add .
git commit -m "Updated README in feature branch"
```

![Feature Commit](screenshots/feature_commit.png)

---

# 10. Pushing Feature Branch

```bash
git push origin feature
```
![Pushing Feature](screenshots/pushing_feature.png)

---

# 11. Compare & Pull Request

![Compare PR](screenshots/pr_featurebranch.png)

---

# 12. Raising Pull Request

![Raised PR](screenshots/raised_prfeature.png)

---

# 13. Creation of Hotfix Branch

```bash
git checkout -b hotfix
```

![Hotfix Branch](screenshots/hotfix_branch.png)

---

# 14. Modified README.md for Hotfix

![Hotfix README](screenshots/changed_hotfixreadme.png)

---

# 15. Commit and Push Hotfix Branch

```bash
git add .
git commit -m "Hotfix changes"
git push origin hotfix
```

![Commit](screenshots/config.png)

![Hotfix Push](screenshots/pushed_hotfix.png)

---

# 16. Compare & Pull Request for Hotfix

![Hotfix PR](screenshots/pr_hotflixbranch.png)

---

# 17. Raising Hotfix Pull Request

![Raised Hotfix PR](screenshots/raised_prhotflix.png)

---

# 18. Git Stash

Commands used:

```bash
git stash
git stash list
git stash pop
```

![Git Stash](screenshots/stash_learning.png)

---

# 19. Merge Feature Branch into Main

```bash
git checkout main
git merge feature
```

![Merged Branch](screenshots/merged_featurebranch.png)

---

# 20. Cherry Pick

```bash
git log --oneline
git cherry-pick <commit-id>
```

![Cherry Pick](screenshots/cherry_picking.png)

---

# 21. Git Rebase

```bash
git rebase main
```

![Rebase](screenshots/rebase.png)

---

# 22. Creating and Pushing Tag

```bash
git tag v1.0
git push origin v1.0
```

![Tag Created](screenshots/tag_created.png)

---

# Conclusion

This assignment provided practical experience with the following Git concepts:

- Repository Forking
- Repository Cloning
- Branch Creation and Switching
- Git Status
- Staging Changes
- Commit and Push
- Pull Requests
- Git Stash
- Merge
- Cherry Pick
- Rebase
- Tag Creation and Push

The assignment helped in understanding a complete Git workflow commonly followed in collaborative software development projects.