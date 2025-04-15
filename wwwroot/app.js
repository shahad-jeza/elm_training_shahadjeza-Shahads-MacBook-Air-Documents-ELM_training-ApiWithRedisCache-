document.addEventListener('DOMContentLoaded', function() {
  const usersContainer = document.getElementById('users-container');
  const refreshBtn = document.getElementById('refresh-btn');
  
  // Initial load
  fetchUsers();
  
  // Set up refresh button
  refreshBtn.addEventListener('click', fetchUsers);

  function setupLazyLoading() {
    const lazyImages = document.querySelectorAll('img[loading="lazy"]');
    
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.addEventListener('load', () => {
                        img.classList.add('loaded');
                    });
                    observer.unobserve(img);
                }
            });
        });


        lazyImages.forEach(img => imageObserver.observe(img));
    } else {
        // Fallback for browsers without IntersectionObserver
        lazyImages.forEach(img => {
            img.src = img.dataset.src;
        });
    }
}
  
  async function fetchUsers() {
      try {
          // Show loading state
          usersContainer.innerHTML = '<p class="loading">Loading user data...</p>';
          // Fetch data WITHOUT reloading the page (AJAX request)
          const response = await fetch('http://localhost:5006/users');
          
          if (!response.ok) {
              throw new Error(`HTTP error! status: ${response.status}`);
          }
          
          const users = await response.json();
          
           // Update ONLY the users container (no full page reload)
          if (users && users.length > 0) {
              displayUsers(users);
          } else {
              usersContainer.innerHTML = '<p class="error">No users found</p>';
          }
      } catch (error) {
          console.error('Error fetching users:', error);
          usersContainer.innerHTML = `<p class="error">Error loading data: ${error.message}</p>`;
      }
  }
  
  function displayUsers(users) {
      usersContainer.innerHTML = '';
      
      users.forEach(user => {
          const userCard = document.createElement('div');
          userCard.className = 'user-card';
          
          userCard.innerHTML = `
              <img 
              src="https://picsum.photos/300/200?random=${user.id}"
              <br>
              <h3>${user.name}</h3>
              <p><strong>Username:</strong> ${user.username}</p>
              <p><strong>Email:</strong> ${user.email}</p>
              <p><strong>Phone:</strong> ${user.phone}</p>
              <p><strong>Website:</strong> <a href="http://${user.website}" target="_blank">${user.website}</a></p>
              <p><strong>Address:</strong> ${user.address.street}, ${user.address.city}, ${user.address.zipcode}</p>
              <p><strong>Company:</strong> ${user.company.name}</p>
          `;
          
          usersContainer.appendChild(userCard);
          setupLazyLoading();
      });
  }
});