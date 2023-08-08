import git
import logging
import argparse

# Set up logging to console
logging.basicConfig(level=logging.INFO)

# Define the order of branches for graduation
BRANCHES = ['dev', 'main', 'candidate'] # 'live' is disabled as it should be a manual graduation after manual testing

repo = git.Repo('.')

def get_current_branch():
    return str(repo.active_branch)

def branch_exists(branch_name):
    return branch_name in repo.branches

def fetch_branch(branch_name):
    try:
        repo.git.fetch('origin', branch_name)
        logging.info(f"Fetched branch {branch_name} from remote")
        if not branch_name in [str(b) for b in repo.branches] and f'origin/{branch_name}' in repo.remotes.origin.refs:
            repo.git.checkout('-b', branch_name, f'origin/{branch_name}')
            logging.info(f"Checked out branch {branch_name} tracking remote branch origin/{branch_name}")
    except git.GitCommandError as e:
        logging.error(f"Failed to fetch branch {branch_name} from remote: {str(e)}")
        raise

def create_branch(branch_name):
    try:
        repo.git.checkout('-b', branch_name)
        logging.info(f"Created new branch {branch_name}")
    except git.GitCommandError as e:
        logging.error(f"Failed to create new branch {branch_name}: {str(e)}")
        raise

def checkout_branch(branch_name):
    try:
        repo.git.checkout(branch_name)
        logging.info(f"Checked out branch {branch_name}")
    except git.GitCommandError as e:
        logging.error(f"Failed to checkout branch {branch_name}: {str(e)}")
        raise

def pull_changes(branch_name):
    try:
        repo.git.fetch('origin', branch_name)
        repo.git.pull('origin', branch_name)
        logging.info(f"Pulled latest changes for branch {branch_name}")
    except git.GitCommandError as e:
        logging.error(f"Failed to pull changes for branch {branch_name}: {str(e)}")
        raise

def merge_branches(source_branch, target_branch):
    try:
        repo.git.merge(source_branch, '--allow-unrelated-histories')
        logging.info(f"Merged branch {source_branch} into {target_branch}")
    except git.GitCommandError as e:
        logging.error(f"Failed to merge branch {source_branch} into {target_branch}: {str(e)}")
        raise

def push_changes(branch_name):
    try:
        push_output = repo.git.push('origin', branch_name, with_extended_output=True)
        logging.info(f"Pushed changes for branch {branch_name}. Output: {push_output}")
    except git.GitCommandError as e:
        logging.error(f"Failed to push changes for branch {branch_name}: {str(e)}")
        raise


def main(branch):
    if branch not in BRANCHES:
        logging.error(f"Branch {branch} is not in the list of branches for graduation")
        return
    if branch == BRANCHES[-1]:
        logging.info("Branch is the last in the graduation list. Nothing to graduate.")
        return
    try:
        next_branch = BRANCHES[BRANCHES.index(branch) + 1]
        pull_changes(branch)
        if not branch_exists(next_branch):
            fetch_branch(next_branch)
            if not branch_exists(next_branch):
                create_branch(next_branch)
        checkout_branch(next_branch)
        pull_changes(next_branch)
        checkout_branch(branch)
        merge_branches(next_branch, branch)
        checkout_branch(next_branch)
        merge_branches(branch, next_branch)
        push_changes(next_branch)
        print('Graduation process completed successfully!')
    except Exception as e:
        logging.error(f"Script failed: {str(e)}")
        raise

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Graduate a branch to the next stage.')
    parser.add_argument('--branch', type=str, help='The branch to graduate. If not provided, the current branch is used.')
    args = parser.parse_args()
    branch = args.branch if args.branch else get_current_branch()
    main(branch)
