(() => {
    const form = document.getElementById('chatForm');
    const messageInput = document.getElementById('messageInput');
    const sendButton = document.getElementById('sendButton');
    const chatHistory = document.getElementById('chatHistory');
    const loading = document.getElementById('loading');
    const conversationIdInput = document.getElementById('conversationId');

    if (!form || !messageInput || !chatHistory || !loading || !conversationIdInput) {
        return;
    }

    const appendMessage = (sender, content) => {
        const wrapper = document.createElement('div');
        wrapper.className = sender === 'User' ? 'message user' : 'message ai';

        const bubble = document.createElement('div');
        bubble.className = 'bubble';
        bubble.textContent = content;

        wrapper.appendChild(bubble);
        chatHistory.appendChild(wrapper);
        chatHistory.scrollTop = chatHistory.scrollHeight;
    };

    const setLoading = (isLoading) => {
        loading.classList.toggle('d-none', !isLoading);
        sendButton.disabled = isLoading;
        messageInput.disabled = isLoading;
    };

    chatHistory.scrollTop = chatHistory.scrollHeight;

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        const message = messageInput.value.trim();
        if (!message) {
            return;
        }

        appendMessage('User', message);
        messageInput.value = stringEmpty();
        setLoading(true);

        try {
            const response = await fetch('/Chat/SendMessage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    conversationId: Number(conversationIdInput.value || '0'),
                    message
                })
            });

            const data = await response.json();
            if (!response.ok || !data.success) {
                appendMessage('AI', data.error || 'Could not get a response from the AI service.');
                return;
            }

            conversationIdInput.value = data.conversationId;
            appendMessage('AI', data.reply);
        } catch {
            appendMessage('AI', 'Could not get a response from the AI service.');
        } finally {
            setLoading(false);
            messageInput.focus();
        }
    });

    function stringEmpty() {
        return '';
    }
})();
