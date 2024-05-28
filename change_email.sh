git filter-branch --force --env-filter '

# 기존 이메일
OLD_EMAIL_JAE="kjhstar971124@gmail.com"
OLD_EMAIL_HYUN="ggobug94@gmail.com"
OLD_EMAIL_JUN="ssafy10go@gmail.com"

# 수정 후 이름 및 이메일
CORRECT_NAME_JAE="김재형"
CORRECT_EMAIL_JAE="kjhstar971124@gmail.com"

CORRECT_NAME_HYUN="최현기"
CORRECT_EMAIL_HYUN="ggobug94@gmail.com"

CORRECT_NAME_JUN="황준식"
CORRECT_EMAIL_JUN="ssafy10go@gmail.com"

# 김재형
if [ "$GIT_COMMITTER_NAME" = "$OLD_EMAIL_JAE" ]
then
    export GIT_COMMITTER_NAME="$CORRECT_NAME_JAE"
    export GIT_COMMITTER_EMAIL="$CORRECT_EMAIL_JAE"
fi
if [ "$GIT_AUTHOR_NAME" = "$OLD_EMAIL_JAE" ]
then
    export GIT_AUTHOR_NAME="$CORRECT_NAME_JAE"
    export GIT_AUTHOR_EMAIL="$CORRECT_EMAIL_JAE"
fi

# 최현기
if [ "$GIT_COMMITTER_NAME" = "$OLD_EMAIL_HYUN" ]
then
    export GIT_COMMITTER_NAME="$CORRECT_NAME_HYUN"
    export GIT_COMMITTER_EMAIL="$CORRECT_EMAIL_HYUN"
fi
if [ "$GIT_AUTHOR_NAME" = "$OLD_EMAIL_HYUN" ]
then
    export GIT_AUTHOR_NAME="$CORRECT_NAME_HYUN"
    export GIT_AUTHOR_EMAIL="$CORRECT_EMAIL_HYUN"
fi

# 황준식
if [ "$GIT_COMMITTER_NAME" = "$OLD_EMAIL_JUN" ]
then
    export GIT_COMMITTER_NAME="$CORRECT_NAME_JUN"
    export GIT_COMMITTER_EMAIL="$CORRECT_EMAIL_JUN"
fi
if [ "$GIT_AUTHOR_NAME" = "$OLD_EMAIL_JUN" ]
then
    export GIT_AUTHOR_NAME="$CORRECT_NAME_JUN"
    export GIT_AUTHOR_EMAIL="$CORRECT_EMAIL_JUN"
fi
' --tag-name-filter cat -- --branches --tags